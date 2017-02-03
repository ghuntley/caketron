using CakeTron.Core.Contexts;
using CakeTron.Core.Parts;

namespace CakeTron.Parts
{
    public sealed class BotSnackPart : CommandPart
    {
        public override string Help => "Give the bot some food.";

        public BotSnackPart()
            : base(new[] { "botsnack" })
        {
        }

        protected override void HandleCommand(MessageContext context, string[] args)
        {
            context.Broadcast("Yum! Thank you very much!");
        }
    }
}