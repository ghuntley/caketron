using System.Net.Http;
using System.Threading;
using CakeTron.Core.Contexts;
using CakeTron.Core.Domain;

namespace CakeTron.Core
{
    public sealed class StreamListenerContext : RoomContext
    {
        public HttpClient HttpClient { get; }
        public CancellationToken Token { get; }

        public StreamListenerContext(HttpClient client, GitterRoom room, GitterUser bot, CancellationToken token)
            : base(new GitterClient(client), bot, room)
        {
            HttpClient = client;
            Token = token;
        }
    }
}