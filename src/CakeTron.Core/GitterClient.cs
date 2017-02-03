using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CakeTron.Core.Domain;
using Newtonsoft.Json;

namespace CakeTron.Core
{
    public sealed class GitterClient
    {
        private readonly HttpClient _client;

        public GitterClient(HttpClient client)
        {
            _client = client;
        }

        public async Task<IEnumerable<GitterRoom>> GetRooms()
        {
            var result = await Get<IEnumerable<GitterRoom>>("https://api.gitter.im/v1/rooms").ConfigureAwait(false);
            return result.ToArray();
        }

        public async Task<GitterUser> GetCurrentUser()
        {
            var result = await Get<IEnumerable<GitterUser>>("https://api.gitter.im/v1/user").ConfigureAwait(false);
            return result.SingleOrDefault();
        }

        public async Task Broadcast(GitterRoom room, string text)
        {
            var message = new GitterMessage { Text = text };
            await Post($"https://api.gitter.im/v1/rooms/{room.Id}/chatMessages", message);
        }

        internal async Task Reply(GitterRoom room, GitterUser fromUser, string text)
        {
            var message = new GitterMessage { Text = $"@{fromUser.Username}: {text}" };
            await Post($"https://api.gitter.im/v1/rooms/{room.Id}/chatMessages", message);
        }

        private async Task<T> Get<T>(string url)
        {
            var response = await _client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(content);
        }

        public async Task Post<T>(string url, T message)
        {
            var json = JsonConvert.SerializeObject(message, Formatting.None, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
            });
            await _client.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));
        }
    }
}
