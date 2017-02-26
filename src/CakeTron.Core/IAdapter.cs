using System.Threading;
using System.Threading.Tasks;

namespace CakeTron.Core
{
    public interface IAdapter
    {
        IBroker Broker { get; }

        Task Start(CancellationTokenSource source);
    }
}
