using CakeTron.Core.Domain;

namespace CakeTron.Parts.Karma
{
    public interface IKarmaProvider
    {
        int Increase(GitterRoom room, string name);
        int Decrease(GitterRoom room, string name);
    }
}