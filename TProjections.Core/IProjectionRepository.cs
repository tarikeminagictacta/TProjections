using System.Threading.Tasks;

namespace TProjections.Core
{
    public interface IProjectionRepository
    {
        Task<long> GetCurrentSequenceAsync();
        Task SaveChangesAsync();
        Task DeleteAllAsync();
    }
}