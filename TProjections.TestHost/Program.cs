using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TProjections.Core;
using TProjections.TestHost.Sample.Projections;
using TProjections.TestHost.Sample.Repositories;

namespace TProjections.TestHost
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    services.AddSingleton<IProjection, BankAccountBalanceProjection>();
                    services.AddSingleton<IBankAccountBalanceRepository, BankAccountBalanceRepository>();
                });
        }
    }
}