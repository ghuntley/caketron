namespace CakeTron.Core
{
    public interface IMessageQueue
    {
        void Enqueue(IEvent @event);
    }
}