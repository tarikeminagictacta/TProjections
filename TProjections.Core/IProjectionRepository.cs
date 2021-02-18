using System.Collections.Generic;
using System.Threading.Tasks;

namespace TProjections.Core
{
    public interface IProjectionRepository
    {
        Task<long> GetCurrentSequenceAsync();
        Task<ICollection<EventEnvelope>> GetEvents(long latestSequence, int eventPageSize);
        Task SaveChangesAsync();
    }
}