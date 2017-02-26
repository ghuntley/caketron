namespace CakeTron.Core
{
    public interface IEvent
    {
        void Accept(IEventDispatcher dispatcher);
    }
}
