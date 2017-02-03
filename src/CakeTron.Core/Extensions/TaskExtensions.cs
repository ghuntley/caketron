using System.Threading;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace CakeTron
{
    internal static class TaskExtensions
    {
        // http://stackoverflow.com/a/28626769
        public static Task<T> WithCancellation<T>(this Task<T> task, CancellationToken cancellationToken)
        {
            return task.IsCompleted
                ? task
                : task.ContinueWith(
                    completedTask => completedTask.GetAwaiter().GetResult(),
                    cancellationToken,
                    TaskContinuationOptions.ExecuteSynchronously,
                    TaskScheduler.Default);
        }
    }
}
