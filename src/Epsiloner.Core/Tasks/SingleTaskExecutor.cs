using System;
using System.Threading;
using System.Threading.Tasks;

namespace Epsiloner.Tasks
{
    /*
     Reason:
     - Sometimes we need to have only 1 running task at moment.
     -> It means that when new task appears, previous should be canceled.
     -> Try to provide to public Task that will be completed even in this scenario:
     - 1. Set task A.
     - 2. wait for Task from outside.
     - 3. Set task to B.
     - 4. task A cancels.
     - 5. outside - still waits for Task completion.
     - 6. task B completes
     - 7. outside - Task (B) completed.

     */

    /// <summary>
    /// TODO: Add documentation.
    /// </summary>
    public class SingleTaskExecutor<TResult> : IDisposable
    {
        private readonly Func<CancellationToken> _tokenResolver;
        private CancellationTokenSource _tokenSource;
        private TaskCompletionSource<TResult> _taskSource = new TaskCompletionSource<TResult>();

        /// <summary>
        /// TODO: Add documentation.
        /// </summary>
        public Task<TResult> Task => _taskSource.Task;

        /// <summary>
        /// TODO: Add documentation.
        /// </summary>
        /// <param name="tokenResolver">(Optional) Method to resolve token for each function executed via <see cref="Next"/>. If not specified will be used <see cref="CancellationToken.None"/>.</param>
        public SingleTaskExecutor(Func<CancellationToken> tokenResolver = null)
        {
            _tokenResolver = tokenResolver ?? (() => CancellationToken.None);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _tokenSource?.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="func"></param>
        /// <returns>Returns <see cref="System.Threading.Tasks.Task"/> retrieved from <paramref name="func"/>.</returns>
        public Task Next(Func<CancellationToken, Task<TResult>> func)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));

            if (_tokenSource != null)
            {
                _tokenSource.Cancel();
                _tokenSource.Dispose();
            }

            var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(_tokenResolver());
            var token = linkedSource.Token;
            _tokenSource = linkedSource;

            var task = func(token);
            if (task == null)
                throw new ArgumentException("Function must return instance of Task.", nameof(func));

            //Check if new TaskCompletionSource should be created.
            if (_taskSource.Task.IsCanceled ||
                _taskSource.Task.IsCompleted ||
                _taskSource.Task.IsFaulted)
                _taskSource = new TaskCompletionSource<TResult>();

            task.ContinueWith(x =>
            {
                if (ReferenceEquals(linkedSource, _tokenSource))
                    _taskSource.TrySetResult(x.Result);
            }, token, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Current).ConfigureAwait(false);
            task.ContinueWith(x =>
            {
                if (ReferenceEquals(linkedSource, _tokenSource))
                    _taskSource.TrySetException(x.Exception ?? new Exception());
            }, token, TaskContinuationOptions.OnlyOnFaulted, TaskScheduler.Current).ConfigureAwait(false);
            task.ContinueWith(x =>
            {
                if (ReferenceEquals(linkedSource, _tokenSource))
                    _taskSource.TrySetCanceled();
            }, token, TaskContinuationOptions.OnlyOnCanceled, TaskScheduler.Current).ConfigureAwait(false);

            return task;
        }
    }
}
