using System;
using System.Threading.Tasks;
using System.Timers;

namespace Epsiloner.Cooldowns
{
    /// <summary>
    /// Postpones event execution after certain time.
    /// It waits for silence gap after last event has been raised.
    /// </summary>
    public class EventCooldown : DisposableObject
    {
        private readonly TimeSpan _accumulateAfter;
        private readonly Action _action;
        private readonly TimeSpan? _maxAccumulateAfter;
        private readonly object _padlock = new object();
        private bool _timerIsDisposed;
        private bool _timerMaxElapsing;

        private Timer _timer;
        private Timer _timerMax;

        /// <summary>
        /// Creates event cooldown.  
        /// </summary>
        /// <param name="accumulateAfter">Timespan after last event execute action.</param>
        /// <param name="action">Action to invoke.</param>
        public EventCooldown(TimeSpan accumulateAfter, Action action)
        : this(accumulateAfter, action, null)
        { }

        /// <summary>
        /// Creates event cooldown.  
        /// </summary>
        /// <param name="accumulateAfter">Timespan after last event execute action.</param>
        /// <param name="action">Action to invoke.</param>
        /// <param name="maxAccumulateAfter">(Optional) Maximum timespan after first event execute action.</param>
        public EventCooldown(TimeSpan accumulateAfter, Action action, TimeSpan? maxAccumulateAfter = null)
        {
            _accumulateAfter = accumulateAfter;
            _action = action;
            _maxAccumulateAfter = maxAccumulateAfter;
            _timer = NewTimer();
            _timerMax = NewMaxTimer();
            IsNow = false;
        }

        /// <summary>
        /// Provides stack trace of last <see cref="Accumulate"/> and <see cref="Now"/>.
        /// </summary>
        public string LastStackTrace { get; private set; }

        /// <summary>
        /// For debugging purposes keeps last stack trace of execution. 
        /// Effective only in DEBUG release mode.
        /// Reduces performance when set to <see langword="true" />.
        /// </summary>
        public bool KeepLastStackTrace { get; set; }

        /// <summary>
        /// Was last cooldown called by now?
        /// </summary>
        public bool IsNow { get; private set; }

        /// <summary>
        /// Executes event with no cooldown. 
        /// </summary>
        public void Now()
        {
            IsNow = true;
            if (IsDisposing)
                return;

            lock (_padlock)
            {
                if (_timer == null)
                    return;

                _timer.Stop();
            }

            InvokeAction();
        }

        /// <summary>
        /// Executes event with no cooldown asynchronously.
        /// </summary>
        /// <returns></returns>
        public Task NowAsync()
        {
            if (KeepLastStackTrace)
                LastStackTrace = Environment.StackTrace;

            return Task.Factory.StartNew(Now);
        }

        /// <summary>
        /// Puts event in cooldown. 
        /// In case no more events comes then OnElapsed will be called.
        /// </summary>
        public void Accumulate()
        {
            IsNow = false;
            if (IsDisposing)
                return;

            lock (_padlock)
            {
                if (_timer == null)
                    return;

                if (KeepLastStackTrace)
                    LastStackTrace = Environment.StackTrace;

                StopStart();
                StartMax();
            }
        }

        /// <summary>
        /// Cancels accumulation.
        /// </summary>
        public void Cancel()
        {
            lock (_padlock)
            {
                try
                {
                    _timer?.Stop();
                    _timerMax?.Stop();
                }
                catch (ObjectDisposedException)
                {
                    _timer = null;
                    _timerMax = null;
                    // ignore, we know this
                }
            }
        }

        private void StopStart()
        {
            try
            {
                _timer?.Stop();
                if (!_timerIsDisposed)
                    _timer?.Start();
            }
            catch (ObjectDisposedException)
            {
                _timer = null;
                // ignore, we know this
            }
        }

        public void StartMax()
        {
            if (_timerMax == null)
                return;

            try
            {
                if (_timerIsDisposed)
                    return;

                if (_timerMaxElapsing)
                    return;

                _timerMaxElapsing = true;
                _timerMax?.Start();
            }
            catch (ObjectDisposedException)
            {
                _timerMax = null;
                // ignore, we know this
            }
        }

        private Timer NewTimer()
        {
            var timer = new Timer(_accumulateAfter.TotalMilliseconds)
            {
                AutoReset = false
            };

            timer.Elapsed += OnElapsed;
            timer.Disposed += TimerDisposed;

            return timer;
        }

        private Timer NewMaxTimer()
        {
            if (!_maxAccumulateAfter.HasValue)
                return null;

            var timer = new Timer(_maxAccumulateAfter.Value.TotalMilliseconds)
            {
                AutoReset = false
            };
            timer.Elapsed += OnMaxElapsed;
            timer.Disposed += TimerDisposed;

            return timer;
        }

        private void OnMaxElapsed(object sender, ElapsedEventArgs e)
        {
            Cancel();
            InvokeAction();
        }

        private void TimerDisposed(object sender, EventArgs e)
        {
            _timerIsDisposed = true;
        }

        private void OnElapsed(object sender, ElapsedEventArgs e)
        {
            InvokeAction();
        }

        private void InvokeAction()
        {
            if (IsDisposing || _timerIsDisposed)
                return;

            _timerMaxElapsing = false;
            _timerMax?.Stop();

            _action.Invoke();
        }

        /// <inheritdoc />
        protected override void DisposeManagedResources()
        {
            lock (_padlock)
            {
                if (_timer == null)
                    return;

                _timer.Close();
                _timer.Elapsed -= OnElapsed;
                _timer.Disposed -= TimerDisposed;
                _timer = null;

                if (_timerMax != null)
                {
                    _timerMax.Close();
                    _timerMax.Elapsed -= OnMaxElapsed;
                    _timerMax.Disposed -= TimerDisposed;
                    _timerMax = null;
                }
            }
        }
    }
}
