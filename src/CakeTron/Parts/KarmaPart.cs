using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using CakeTron.Core;
using CakeTron.Core.Contexts;
using CakeTron.Core.Domain;

namespace CakeTron.Parts
{
    internal sealed class KarmaPart : RobotPart
    {
        private readonly Brain _brain;
        private readonly Regex _regex;
        private readonly Random _random;
        private readonly object _lock;

        private Dictionary<string, Dictionary<string, int>> _state;

        private readonly string[] _incrementResponses = { "+1!", "gained a level!", "is on the rise!", "leveled up!" };
        private readonly string[] _decrementResponses = { "took a hit! Ouch.", "took a dive.", "lost a life.", "lost a level." };

        public KarmaPart(Brain brain)
        {
            _brain = brain;
            _regex = new Regex("^(?<thing>@?[a-zA-Z0-9\\d](?:[a-zA-Z0-9\\d]|-(?=[a-zA-Z0-9\\d])){0,38})(?<op>\\+\\+|\\-\\-)", RegexOptions.Compiled);
            _random = new Random(DateTime.Now.Millisecond);
            _lock = new object();
        }

        public override void Initialize()
        {
            // Try getting the data
            var data = _brain.Get<Dictionary<string, Dictionary<string, int>>>("Karma");
            data = data ?? new Dictionary<string, Dictionary<string, int>>();

            // Set the internal state.
            _state = new Dictionary<string, Dictionary<string, int>>(data, StringComparer.OrdinalIgnoreCase);
        }

        public override bool Handle(MessageContext context)
        {
            var match = _regex.Match(context.Message.Text);
            if (match.Success)
            {
                var name = match.Groups["thing"].Value;

                var op = match.Groups["op"].Value;
                if (op == "++")
                {
                    var karma = Increase(context.Room, name);
                    var response = _incrementResponses[_random.Next(0, _incrementResponses.Length)];
                    context.Broadcast($"{name} {response} (Karma: {karma})");
                }
                else
                {
                    var karma = Decrease(context.Room, name);
                    var response = _decrementResponses[_random.Next(0, _decrementResponses.Length)];
                    context.Broadcast($"{name} {response} (Karma: {karma})");
                }
                return true;
            }
            return false;
        }

        private int Increase(Room room, string name)
        {
            return Update(room, name, karma => karma + 1);
        }

        private int Decrease(Room room, string name)
        {
            return Update(room, name, karma => karma - 1);
        }

        private int Update(Room room, string name, Func<int, int> func)
        {
            lock (_lock)
            {
                if (!_state.ContainsKey(room.Id))
                {
                    _state.Add(room.Id, new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase));
                }
                if (!_state[room.Id].ContainsKey(name))
                {
                    _state[room.Id].Add(name, 0);
                }
                
                // Update karma
                var karma = func(_state[room.Id][name]);
                _state[room.Id][name] = karma;

                // Save the state.
                _brain.Set("Karma", _state);

                return karma;
            }
        }
    }
}
