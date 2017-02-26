using Newtonsoft.Json;

namespace CakeTron.Slack.Models
{
    internal class SlackEvent
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
    }
}
