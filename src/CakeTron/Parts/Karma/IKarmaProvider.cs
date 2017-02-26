using CakeTron.Core.Domain;

namespace CakeTron.Parts.Karma
{
    public interface IKarmaProvider
    {
        int Increase(Room room, string name);
        int Decrease(Room room, string name);
    }
}