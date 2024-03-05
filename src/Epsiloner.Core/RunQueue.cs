using System;
using System.Threading;
using System.Threading.Tasks;

namespace Epsiloner
{
    /// <summary>
    /// Provides functionality to run action only once at time and in case the same action
    /// during execution is invoked multiple times - it will put invocations into queue and will run them in async.
    /// </summary>
    public class RunQueue : DisposableObject
    {
        private readonly int _queueMax;

        private readonly object _lock = new object();
        private readonly SemaphoreSlim _semaphore;
        private readonly CancellationTokenSource _tokenSource;
        private readonly CancellationToken _token;

        /// <summary>
        /// Action to invoke via <see cref="RunAsync"/>.
        /// </summary>
        public Action Action { get; }

        /// <summary>
        /// Constructor for <see cref="RunQueue"/>.
        /// </summary>
        /// <param name="action">Action that will be invoked via <see cref="RunAsync"/>. Weak reference.</param>
        /// <param name="queueLimit">Queue limit.</param>
        public RunQueue(Action action, int queueLimit = 1)
        {
            if (queueLimit < 1)
                throw new ArgumentException("Queue limit must be >= 1.");

            _queueMax = queueLimit + 1;
            _semaphore = new SemaphoreSlim(_queueMax, _queueMax);
            _tokenSource = new CancellationTokenSource();
            _token = _tokenSource.Token;

            Action = action ?? throw new ArgumentNullException(nameof(action));
        }

        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();
            _tokenSource.Cancel();
            lock (_lock)
                _semaphore.Dispose();
        }

        /// <summary>
        /// Runs if queue is empty or adds to queue if queue is not full. Otherwise nothing happens.
        /// </summary>
        public async Task RunAsync()
        {
            if (IsDisposed)
                return;

            try
            {
                lock (_lock)
                {
                    if (_semaphore.CurrentCount == 0) // Currently running + full queue.
                        return;

                    if (_semaphore.CurrentCount < _queueMax) // Currently running, queue is not full.
                    {
                        _semaphore.Wait(_token); // Put 1 run into queue.
                        return;
                    }

                    _semaphore.Wait(_token); // Start run without queue.
                }

                var runOnceMore = false;
                if (_tokenSource.IsCancellationRequested)
                    return;
                try
                {
                    Action.Invoke();
                }
                finally
                {
                    if (!IsDisposed && !_tokenSource.IsCancellationRequested)
                    {
                        lock (_lock)
                        {
                            _semaphore.Release(); // Finish current run.
                            if (_semaphore.CurrentCount != _queueMax) // Check if anything left in queue.
                            {
                                _semaphore.Release();
                                runOnceMore = true;
                            }
                        }

                        if (runOnceMore)
                            await RunAsync();
                    }
                }
            }
            catch (OperationCanceledException) // Catch when RunQueue is disposed and someone waits in queue.
            {
                // ignore.
            }
        }

        /// <summary>
        /// Synchronous version of <see cref="RunAsync"/>.
        /// </summary>
        public void Run() => RunAsync().Wait(_token);
    }
}
