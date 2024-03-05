using System;
using System.Threading.Tasks;
using System.Timers;

namespace Epsiloner.Cooldowns
{
    /// <summary>
    /// Postpones event execution after certain time.
    /// It waits for silence gap after last event has been raised.
    /// </summary>
    public class EventCooldown<T> : DisposableObject, IEventCooldown<T>
    {
        private readonly TimeSpan _accumulateAfter;
        private readonly Action<T> _action;
        private readonly TimeSpan? _maxAccumulateAfter;
        private readonly object _padlock = new object();
        private bool _timerIsDisposed;
        private bool _timerMaxElapsing;

        private T _value;
        private Timer _timer;
        private Timer _timerMax;

        /// <summary>
        /// Creates event cooldown.  
        /// </summary>
        /// <param name="accumulateAfter">Timespan after last event execute action.</param>
        /// <param name="action">Action to invoke.</param>
        public EventCooldown(TimeSpan accumulateAfter, Action<T> action)
        : this(accumulateAfter, action, null)
        { }

        /// <summary>
        /// Creates event cooldown.  
        /// </summary>
        /// <param name="accumulateAfter">Timespan after last event execute action.</param>
        /// <param name="action">Action to invoke.</param>
        /// <param name="maxAccumulateAfter">(Optional) Maximum timespan after first event execute action.</param>
        public EventCooldown(TimeSpan accumulateAfter, Action<T> action, TimeSpan? maxAccumulateAfter = null)
        {
            _accumulateAfter = accumulateAfter;
            _action = action;
            _maxAccumulateAfter = maxAccumulateAfter;
            _timer = NewTimer();
            _timerMax = NewMaxTimer();
            IsNow = false;
        }

        /// <inheritdoc />
        public string LastStackTrace { get; private set; }

        /// <inheritdoc />
        public bool IsNow { get; private set; }

        /// <inheritdoc />
        public bool KeepLastStackTrace { get; set; }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public void Now(T value)
        {
            IsNow = true;
            if (IsDisposing)
                return;

            lock (_padlock)
            {
                if (_timer == null)
                    return;

                _timer.Stop();
                _value = value;
            }
            InvokeAction();
        }

        /// <inheritdoc />
        public void NowAsync(T value)
        {
            Task.Factory.StartNew(() => Now(value));
        }

        /// <inheritdoc />
        public void Cancel()
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

        /// <inheritdoc />
        public void Accumulate(T value)
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

                StopStart(value);
                StartMax();
            }
        }

        public bool Any()
        {
            lock (_padlock)
            {
                return !_timerIsDisposed && _timer?.Enabled == true;
            }
        }

        protected virtual void StopStart(T value)
        {
            try
            {
                _timer?.Stop();
                _value = value;
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

            _action.Invoke(_value);
        }

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
