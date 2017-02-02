namespace CakeTron.Core
{
    public interface IEvent
    {
        void Accept(EventDispatcher dispatcher);
    }
}
