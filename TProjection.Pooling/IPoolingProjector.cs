using System.Threading.Tasks;

namespace TProjection.Pooling
{
    public interface IPoolingProjector
    {
        Task SetSequence();
        Task StartProjecting();
        Task StopProjecting();
    }
}