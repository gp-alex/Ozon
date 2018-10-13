using DomainModels;
using Infrastructure;
using InfrastructureEf.EntityFramework;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InfrastructureEf.Repositories
{
    public class EfPairRepository : IPairRepository
    {
        private readonly SharedContext sharedContext;
        private readonly ILogger log;
        public EfPairRepository(
            SharedContext sharedContext,
            ILogger log
        )
        {
            this.sharedContext = sharedContext ?? throw new ArgumentNullException("sharedContext");
            this.log = log?.ForContext(GetType());
        }

        public async Task<IEnumerable<Pair>> FindAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Pair>> FindAllInYearAsync(int year)
            => throw new NotImplementedException("Not required for test task");

        public async Task DeleteAllAsync()
        {
            sharedContext.Pairs.RemoveRange(sharedContext.Pairs);
            await sharedContext.SaveChangesAsync();
        }

        public async Task SaveAllAsync(IEnumerable<Pair> pairs)
        {
            var uniquePairs = pairs
                .Where(
                    x => sharedContext.Pairs.Find(x.Dt, x.BaseSymbol, x.CounterSymbol, x.Rate) == null
                );

            await sharedContext.Pairs.AddRangeAsync(uniquePairs);
            int numSaves = await sharedContext.SaveChangesAsync();
        }
    }
}
