using Newtonsoft.Json;

namespace CakeTron.Slack.Models
{
    internal sealed class SlackChannelCreated : SlackEvent
    {
        [JsonProperty(PropertyName = "channel")]
        public SlackChannel Channel { get; set; }
    }
}
