using System.Threading;
using System.Threading.Tasks;
using CakeTron.Core.Diagnostics;
using CakeTron.Core.Utilities;

namespace CakeTron.Core.Internal
{
    internal sealed class MessageRouter : TaskWrapper
    {
        private readonly MessageQueue _messageQueue;
        private readonly EventDispatcher _dispatcher;

        public override string FriendlyName => "Message router";

        public MessageRouter(MessageQueue messageQueue, EventDispatcher dispatcher, ILog log) 
            : base(log)
        {
            _messageQueue = messageQueue;
            _dispatcher = dispatcher;
        }

        protected override Task<bool> Run(CancellationToken token)
        {
            while (true)
            {
                // Wait for any messages to arrive.
                var message = _messageQueue.Dequeue(token);
                message?.Accept(_dispatcher);

                if (token.IsCancellationRequested)
                {
                    break;
                }
            }

            return Task.FromResult(true);
        }
    }
}
