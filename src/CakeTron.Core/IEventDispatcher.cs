using CakeTron.Core.Events;

namespace CakeTron.Core
{
    public interface IEventDispatcher
    {
        void Visit(MessageEvent @event);
    }
}