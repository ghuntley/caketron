using CakeTron.Core.Domain;

namespace CakeTron.Core.Contexts
{
    public sealed class MessageContext : RoomContext
    {
        public GitterMessage Message { get; }

        public MessageContext(GitterClient client, GitterUser bot, GitterRoom room, GitterMessage message)
            : base(client, bot, room)
        {
            Message = message;
        }

        public void Reply(string message)
        {
            GitterClient.Reply(Room, Message.FromUser, message).Wait();
        }
    }
}
