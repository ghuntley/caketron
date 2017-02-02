using System;
using System.Collections.Generic;
using CakeTron.Core.Contexts;
using CakeTron.Core.Parts;

namespace CakeTron.Parts
{
    public sealed class PodBayDoorsPart : MentionablePart
    {
        private readonly HashSet<string> _phrases;

        public PodBayDoorsPart()
        {
            _phrases = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "open the pod bay doors",
                "open the pod bay door",
                "open the door",
                "open the doors"
            };
        }

        protected override bool HandleMention(MessageContext context, string message)
        {
            foreach (var phrase in _phrases)
            {
                if (message.Contains(phrase, StringComparison.OrdinalIgnoreCase))
                {
                    context.Broadcast($"I'm sorry, {context.Message.FromUser.DisplayName}. I'm afraid I can't do that.");
                    return true;
                }
            }
            return false;
        }
    }
}