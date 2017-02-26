using Newtonsoft.Json;

namespace CakeTron.Slack.Models
{
    internal sealed class SlackTeamJoin : SlackEvent
    {
        [JsonProperty(PropertyName = "user")]
        public SlackUser User { get; set; }
    }
}
