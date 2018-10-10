using DomainModels;
using System.Data.Entity;

namespace InfrastructureEf.EntityFramework
{
    internal class PairContext
    {
        public PairContext()
            : base()
        {
        }

        public DbSet<Pair> Pairs { get; set; }
    }
}
