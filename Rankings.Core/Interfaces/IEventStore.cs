using System.Threading.Tasks;
using Rankings.Core.Aggregates.Games;

namespace Rankings.Core.Interfaces
{
    public interface IEventStore
    {
        Task CreateEvent(SingleGameRegistered singleGameRegistered);
    }
}