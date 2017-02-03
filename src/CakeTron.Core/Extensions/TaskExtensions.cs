using System.Threading;
using System.Threading.Tasks;
using CakeTron.Core.Diagnostics;

// ReSharper disable once CheckNamespace
namespace CakeTron
{
    internal static class TaskExtensions
    {
        // http://stackoverflow.com/a/28626769
        public static Task<T> WithCancellation<T>(this Task<T> task, CancellationToken token)
        {
            if (task.IsCompleted)
            {
                return task;
            }

            return task.ContinueWith(
                completedTask => completedTask.GetAwaiter().GetResult(),
                token,
                TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default);
        }
    }
}
