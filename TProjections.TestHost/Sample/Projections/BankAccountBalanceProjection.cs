using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TProjections.Core;
using TProjections.TestHost.Sample.Events;
using TProjections.TestHost.Sample.Repositories;

namespace TProjections.TestHost.Sample.Projections
{
    public class BankAccountBalanceProjection : Projection
    {
        private readonly ILogger<BankAccountBalanceProjection> _logger;
        private readonly IBankAccountBalanceRepository _repository;

        public BankAccountBalanceProjection(IBankAccountBalanceRepository repository,
            ILogger<BankAccountBalanceProjection> logger)
            : base(repository)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task On(BankAccountCreated @event, long sequence)
        {
            await _repository.RegisterAsync(@event.Id, sequence);
            _logger.LogInformation($"Bank account {_repository.BankAccount} created.");
        }

        public async Task On(MoneyWithdrawn @event, long sequence)
        {
            var balance = await _repository.GetAsync(@event.Id);
            balance.Balance -= @event.Amount;
            await _repository.UpdateBalance(balance, sequence);

            _logger.LogInformation(
                $"Money withdrawn from bank account {_repository.BankAccount}. New Balance: {balance.Balance}");
        }

        public async Task On(MoneyDeposited @event, long sequence)
        {
            var balance = await _repository.GetAsync(@event.Id);
            balance.Balance += @event.Amount;
            await _repository.UpdateBalance(balance, sequence);

            _logger.LogInformation(
                $"Money deposited from bank account {_repository.BankAccount}. New Balance: {balance.Balance}");
        }
    }
}