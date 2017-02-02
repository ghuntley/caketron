using CakeTron.Core.Domain;

namespace CakeTron.Core
{
    public sealed class StreamClientContext
    {
        public GitterUser Bot { get; }
        public GitterRoom[] Rooms { get; }

        public StreamClientContext(GitterUser bot, GitterRoom[] rooms)
        {
            Bot = bot;
            Rooms = rooms;
        }
    }
}