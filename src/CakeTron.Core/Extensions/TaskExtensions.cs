using System;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace CakeTron
{
    internal static class TaskExtensions
    {
        public static async Task<T> WithCancellation<T>(this Task<T> task, CancellationToken cancellationToken)
        {
            using (cancellationToken.Register(cancellationToken.ThrowIfCancellationRequested, true))
            {
                return await task;
            }
        }
    }
}
