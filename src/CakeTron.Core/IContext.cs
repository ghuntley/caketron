using CakeTron.Core.Domain;

namespace CakeTron.Core
{
    public interface IContext
    {
        GitterUser Bot { get; }
        GitterClient Client { get; }
        GitterRoom Room { get; }
    }
}