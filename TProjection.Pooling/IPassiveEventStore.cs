using System.Collections.Generic;
using System.Threading.Tasks;
using TProjections.Core;

namespace TProjection.Pooling
{
    public interface IPassiveEventStore
    {
        Task<ICollection<EventEnvelope>> GetEvents(long latestSequence, int eventPageSize);
    }
}