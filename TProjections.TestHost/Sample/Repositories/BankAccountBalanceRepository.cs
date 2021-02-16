using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TProjections.TestHost.Sample.Repositories
{
    public class BankAccountBalanceRepository : IBankAccountBalanceRepository
    {
        private List<BankAccountBalance> _accountBalances;

        public BankAccountBalanceRepository()
        {
            _accountBalances = new List<BankAccountBalance>();
        }

        public IReadOnlyList<BankAccountBalance> AccountBalances => _accountBalances.AsReadOnly();

        public async Task<long> GetCurrentSequenceAsync()
        {
            return 0;
        }

        public async Task SaveChangesAsync()
        {
            // Do nothing
        }

        public async Task DeleteAllAsync()
        {
            _accountBalances = new List<BankAccountBalance>();
        }

        public async Task RegisterAsync(string eventId, long sequence)
        {
            _accountBalances.Add(new BankAccountBalance {Balance = 0M, Id = eventId, Sequence = sequence});
        }

        public async Task<BankAccountBalance> GetAsync(string eventId)
        {
            return _accountBalances.First(b => b.Id == eventId);
        }

        public async Task UpdateBalance(BankAccountBalance balance, long sequence)
        {
            var existingBalance = await GetAsync(balance.Id);
            _accountBalances.Remove(existingBalance);
            _accountBalances.Add(balance);
        }
    }
}