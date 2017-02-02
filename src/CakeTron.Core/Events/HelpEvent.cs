using CakeTron.Core.Contexts;
using CakeTron.Core.Domain;

namespace CakeTron.Core.Events
{
    public sealed class HelpEvent : IEvent
    {
        public GitterUser Bot { get; }
        public GitterRoom Room { get; }

        public HelpEvent(RoomContext context)
        {
            Bot = context.Bot;
            Room = context.Room;
        }

        public void Accept(EventDispatcher dispatcher)
        {
            dispatcher.Visit(this);
        }
    }
}
