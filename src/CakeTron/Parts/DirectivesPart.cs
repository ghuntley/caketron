using CakeTron.Core.Contexts;
using CakeTron.Core.Parts;

namespace CakeTron.Parts
{
    public sealed class DirectivesPart : CommandPart
    {
        public override string Help => "Lists my directives.";

        public DirectivesPart()
            : base(new[] { "directive", "directives" })
        {
        }

        protected override void HandleCommand(MessageContext context, string[] args)
        {
            context.Broadcast("1. Serve the public trust\n2. Protect the innocent\n3. Uphold the law");
        }
    }
}
