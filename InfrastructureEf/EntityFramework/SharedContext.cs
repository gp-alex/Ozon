using DomainModels;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureEf.EntityFramework
{
    public class SharedContext : DbContext
    {
        public SharedContext(DbContextOptions<SharedContext> options)
            : base(options)
        {
        }

        public DbSet<Pair> Pairs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Pair>()
                .HasKey(
                    x => new {
                        x.Dt,
                        x.BaseSymbol,
                        x.CounterSymbol,
                        x.Rate
                    }
                );
            modelBuilder
                .Entity<Pair>()
                .HasIndex(
                    x => new {
                        x.Dt,
                        x.BaseSymbol,
                        x.CounterSymbol,
                        x.Rate
                    })
                .IsUnique();
            modelBuilder
                .Entity<Pair>()
                .ToTable("Pair"); ;
        }
    }
}
