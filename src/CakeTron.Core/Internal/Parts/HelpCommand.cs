using CakeTron.Core.Contexts;
using CakeTron.Core.Events;
using CakeTron.Core.Utilities;

namespace CakeTron.Core.Internal.Parts
{
    internal sealed class HelpCommand : CommandPart
    {
        private readonly MessageQueue _messageQueue;

        public override string Help => "Displays all of the help commands that I know about.";

        public HelpCommand(MessageQueue messageQueue)
            : base(new [] { "help" })
        {
            _messageQueue = messageQueue;
        }

        protected override void HandleCommand(MessageContext context, string[] args)
        {
            _messageQueue.Enqueue(new HelpEvent(context));
        }
    }
}
