using CakeTron.Core.Domain;

namespace CakeTron.Core
{
    public abstract class Context
    {
        public GitterClient GitterClient { get; }
        public GitterUser Bot { get; }
        
        protected Context(GitterClient client, GitterUser bot)
        {
            GitterClient = client;
            Bot = bot;
        }
    }
}