using System.Threading.Tasks;

namespace Rankings.Core.Commands
{
    public interface IEventStore
    {
        Task CreateEvent(SingleGameRegistered singleGameRegistered);
    }
}