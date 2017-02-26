using System.Threading.Tasks;
using CakeTron.Core.Domain;

namespace CakeTron.Core
{
    public interface IBroker
    {
        Task Reply(Room room, User fromUser, string text);

        Task Broadcast(Room room, string text);
    }
}
