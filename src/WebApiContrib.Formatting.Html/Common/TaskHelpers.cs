using System;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiContrib.Formatting.Html.Common
{
    internal static class TaskHelpers
    {
        private static readonly Task _defaultCompleted = CreateCompleted();


        internal static Task RunSync(Action action, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (cancellationToken.IsCancellationRequested)
                return Canceled();

            try
            {
                action();
                return Completed();
            }
            catch (Exception e)
            {
                return Error(e);
            }
        }


        internal static Task Canceled()
        {
            return CancelCache<AsyncVoid>.Canceled;
        }

        
        internal static Task Completed()
        {
            return _defaultCompleted;
        }


        private static class CancelCache<TResult>
        {
            public static readonly Task<TResult> Canceled = CreateCancelledTask();

            private static Task<TResult> CreateCancelledTask()
            {
                var taskCompletionSource = new TaskCompletionSource<TResult>();
                taskCompletionSource.SetCanceled();
                return taskCompletionSource.Task;
            }
        }


        internal static Task Error(Exception exception)
        {
            var taskCompletionSource = new TaskCompletionSource<AsyncVoid>();
            taskCompletionSource.SetException(exception);
            return taskCompletionSource.Task;
        }


        internal static Task CreateCompleted()
        {
            var taskCompletionSource = new TaskCompletionSource<AsyncVoid>();
            taskCompletionSource.SetResult(default(AsyncVoid));
            return taskCompletionSource.Task;
        }


        private struct AsyncVoid
        {
        }
    }
}
