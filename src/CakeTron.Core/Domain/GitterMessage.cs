using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CakeTron.Core.Domain
{
    public class GitterMessage
    {
        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }
        public DateTime Sent { get; set; }
        public GitterUser FromUser { get; set; }
        public List<string> Urls { get; set; }
        public List<GitterMention> Mentions { get; set; }
        public List<GitterIssue> Issues { get; set; }
    }
}
