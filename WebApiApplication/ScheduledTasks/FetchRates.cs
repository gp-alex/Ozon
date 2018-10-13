using Infrastructure;
using InfrastructureWeb.Repositories;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApiApplication.AspInfrastructure.HostingServices;

namespace WebApiApplication.ScheduledTasks
{
    public class FetchRates : IScheduledTask
    {
        public string Schedule { get; }
        private int count = 1;
        public FetchRates(string schedule)
        {
            this.Schedule = schedule;
        }

        public async Task Invoke(
            IPairRepository pairPersistanceRepo,
            WebPairRepository pairExternalRepo,
            CancellationToken cancellationToken
        )
        {
            System.Diagnostics.Debug.WriteLine("--------------Tick #" + count++);
            var pairs = await pairExternalRepo.FindCurrentAsync();
            var x = pairs.ToArray();
            await pairPersistanceRepo.SaveAllAsync(pairs);
        }
    }
}
