using DomainModels;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureEf.EntityFramework
{
    internal class PairContext : DbContext
    {
        public PairContext()
            : base()
        {
        }

        public DbSet<Pair> Pairs { get; set; }
    }
}
