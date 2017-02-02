using CakeTron.Core.Contexts;
using CakeTron.Core.Events;

namespace CakeTron.Core.Parts
{
    public sealed class HelpCommand : CommandPart
    {
        private readonly IInbox _inbox;

        public override string Help => "Displays all of the help commands that I know about.";

        public HelpCommand(IInbox inbox)
            : base(new [] { "help" })
        {
            _inbox = inbox;
        }

        protected override void HandleCommand(MessageContext context, string[] args)
        {
            _inbox.Enqueue(new HelpEvent(context));
        }
    }
}
