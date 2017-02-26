using System.Threading;
using System.Threading.Tasks;

namespace CakeTron.Core
{
    public interface IWorker
    {
        string FriendlyName { get; }
        Task<bool> Run(CancellationToken token);
    }
}
