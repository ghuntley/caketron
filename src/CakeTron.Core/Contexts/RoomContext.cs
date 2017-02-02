using CakeTron.Core.Domain;

namespace CakeTron.Core.Contexts
{
    public class RoomContext : Context
    {
        public GitterRoom Room { get; }

        public RoomContext(GitterClient client, GitterUser bot, GitterRoom room)
            : base(client, bot)
        {
            Room = room;
        }

        public void Broadcast(string message)
        {
            GitterClient.Broadcast(Room, message).Wait();
        }
    }
}