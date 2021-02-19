using System.Threading.Tasks;
using TProjections.Core;

namespace TProjections.TestHost.Sample.Repositories
{
    public interface IBankAccountBalanceRepository : IProjectionRepository
    {
        int BankAccount { get; }
        Task RegisterAsync(string eventId, long sequence);
        Task<BankAccountBalance> GetAsync(string eventId);
        Task UpdateBalance(BankAccountBalance balance, long sequence);
    }
}