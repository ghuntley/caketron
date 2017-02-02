using CakeTron.Core.Domain;

namespace CakeTron.Core.Events
{
    public sealed class MessageEvent : IEvent
    {
        public GitterUser Bot { get; set; }
        public GitterRoom Room { get; set; }
        public GitterMessage Message { get; set; }

        public void Accept(EventDispatcher dispatcher)
        {
            dispatcher.Visit(this);
        }
    }
}
