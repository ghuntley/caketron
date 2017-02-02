using System.Collections.Generic;
using System.Linq;
using System.Text;
using CakeTron.Core.Contexts;
using CakeTron.Core.Diagnostics;
using CakeTron.Core.Events;
using CakeTron.Core.Parts;

namespace CakeTron.Core
{
    public sealed class EventDispatcher
    {
        private readonly GitterClient _client;
        private readonly ILog _log;
        private readonly List<RobotPart> _messageHandlers;
        private readonly object _lock;

        public EventDispatcher(GitterClient client, ILog log, IEnumerable<RobotPart> handlers)
        {
            _client = client;
            _log = log;
            _messageHandlers = new List<RobotPart>(handlers);
            _lock = new object();
        }

        public void Visit(MessageEvent @event)
        {
            lock (_lock)
            {
                var context = new MessageContext(_client, @event.Bot, @event.Room, @event.Message);
                foreach (var handler in _messageHandlers)
                {
                    if (handler.Handle(context))
                    {
                        _log.Verbose("Handled message {0} in {1}.", @event.Message.Text, @event.Room.Name);
                        break;
                    }
                }
            }
        }

        public void Visit(HelpEvent @event)
        {
            lock (_lock)
            {
                var context = new RoomContext(_client, @event.Bot, @event.Room);

                var commands = new Dictionary<string, string>();
                foreach (var command in _messageHandlers.OfType<CommandPart>())
                {
                    var name = command.Aliases.FirstOrDefault() ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(name.Trim()) && !string.IsNullOrWhiteSpace(command.Help))
                    {
                        commands[name] = command.Help.Trim();
                    }
                }

                var builder = new StringBuilder();
                var index = 1;
                foreach (var command in commands)
                {
                    builder.AppendLine($"{index}. `{command.Key}` {command.Value}");
                    index++;
                }

                context.Broadcast(builder.ToString());
            }
        }
    }
}
