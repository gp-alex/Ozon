using DomainModels;
using Infrastructure;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InfrastructureEf.Repositories
{
    public class EfPairRepository : IPairRepository
    {
        private readonly ILogger log;
        public EfPairRepository(ILogger log)
        {
            this.log = log.ForContext(GetType());
        }

        public async Task<IEnumerable<Pair>> FindAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task SaveAllAsync(IEnumerable<Pair> pairs)
        {

        }
    }
}
