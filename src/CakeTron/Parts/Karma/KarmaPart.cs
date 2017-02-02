using System;
using System.Text.RegularExpressions;
using CakeTron.Core;
using CakeTron.Core.Contexts;
using Humanizer;

namespace CakeTron.Parts.Karma
{
    public sealed class KarmaPart : RobotPart
    {
        private readonly IKarmaProvider _provider;
        private readonly Regex _regex;
        private readonly Random _random;

        private readonly string[] _incrementResponses = new [] { "+1!", "gained a level!", "is on the rise!", "leveled up!" };
        private readonly string[] _decrementResponses = new [] { "took a hit! Ouch.", "took a dive.", "lost a life.", "lost a level." };

        public KarmaPart(IKarmaProvider provider)
        {
            _provider = provider;
            _regex = new Regex("^(?<thing>@?[a-zA-Z0-9\\d](?:[a-zA-Z0-9\\d]|-(?=[a-zA-Z0-9\\d])){0,38})(?<op>\\+\\+|\\-\\-)", RegexOptions.Compiled);
            _random = new Random(DateTime.Now.Millisecond);
        }

        public override bool Handle(MessageContext context)
        {
            var match = _regex.Match(context.Message.Text);
            if (match.Success)
            {
                var name = match.Groups["thing"].Value;
                name = name.StartsWith("@") 
                    ? name.ToLowerInvariant() 
                    : name.ApplyCase(LetterCasing.Sentence);

                var op = match.Groups["op"].Value;
                if (op == "++")
                {
                    var karma = _provider.Increase(context.Room, name);
                    var response = _incrementResponses[_random.Next(0, _incrementResponses.Length)];
                    context.Broadcast($"{name} {response} (Karma: {karma})");
                }
                else
                {
                    var karma = _provider.Decrease(context.Room, name);
                    var response = _decrementResponses[_random.Next(0, _decrementResponses.Length)];
                    context.Broadcast($"{name} {response} (Karma: {karma})");
                }
                return true;
            }
            return false;
        }
    }
}
