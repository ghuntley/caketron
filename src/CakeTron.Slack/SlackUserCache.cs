using System.Collections.Concurrent;
using CakeTron.Core.Domain;
using CakeTron.Slack.Models;

namespace CakeTron.Slack
{
    internal sealed class SlackUserCache
    {
        private readonly ConcurrentDictionary<string, User> _dictionary;

        public SlackUserCache()
        {
            _dictionary = new ConcurrentDictionary<string, User>();
        }

        public void Initialize(SlackHandshake handshake)
        {
            // Clear the dictionary.
            _dictionary.Clear();

            // Add channels
            foreach (var slackUser in handshake.Users)
            {
                var user = new User { Id = slackUser.Id, DisplayName = slackUser.Profile.FirstName, Username = slackUser.Name };
                _dictionary.AddOrUpdate(slackUser.Id, user, (k, v) => user);
            }
        }

        public User GetUser(string id)
        {
            return _dictionary.TryGetValue(id, out User user) ? user : null;
        }

        public void AddUser(SlackUser slackUser)
        {
            var user = new User { Id = slackUser.Id, DisplayName = slackUser.Profile.FirstName, Username = slackUser.Name };
            _dictionary.AddOrUpdate(slackUser.Id, user, (k, v) => user);
        }
    }
}