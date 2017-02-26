using System;
using System.Collections.Generic;
using CakeTron.Core.Domain;

namespace CakeTron.Parts.Karma
{
    public sealed class InMemoryKarmaProvider : IKarmaProvider
    {
        private readonly Dictionary<string, Dictionary<string, int>> _karma;

        public InMemoryKarmaProvider()
        {
            _karma = new Dictionary<string, Dictionary<string, int>>(StringComparer.OrdinalIgnoreCase);
        }

        public int Increase(Room room, string name)
        {
            return Update(room, name, karma => karma + 1);
        }

        public int Decrease(Room room, string name)
        {
            return Update(room, name, karma => karma - 1);
        }

        private int Update(Room room, string name, Func<int, int> operation)
        {
            if (!_karma.ContainsKey(room.Name))
            {
                _karma[room.Name] = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            }
            if (!_karma[room.Name].ContainsKey(name))
            {
                _karma[room.Name][name] = 0;
            }
            _karma[room.Name][name] = operation(_karma[room.Name][name]);
            return _karma[room.Name][name];
        }
    }
}
