using System.Threading;
using System.Threading.Tasks;

namespace TProjections.Core
{
    public interface IProjection
    {
        Task SetSequence();
        Task StartProjecting(CancellationToken cancellationToken);
    }
}