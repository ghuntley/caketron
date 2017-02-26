using System;
using CakeTron.Core.Contexts;
using CakeTron.Core.Utilities;
using Humanizer;
using Humanizer.Localisation;

namespace CakeTron.Parts
{
    public sealed class UptimePart : CommandPart
    {
        private readonly DateTime _started;

        public override string Help => "Outputs how long I've been sentient.";

        public UptimePart()
            : base(new[] { "uptime" })
        {
            _started = DateTime.Now;
        }

        protected override void HandleCommand(MessageContext context, string[] args)
        {
            var elapsed = DateTime.Now - _started;
            context.Broadcast($"I've been sentient for {elapsed.Humanize(5, minUnit: TimeUnit.Second)}.");
        }
    }
}
