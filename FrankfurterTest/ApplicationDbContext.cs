using FrankfurterTest.Entities;
using Microsoft.EntityFrameworkCore;

namespace FrankfurterTest
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Currency>().Property(p => p.Symbol).HasMaxLength(10);
            modelBuilder.Entity<Currency>().Property(p => p.Name).HasMaxLength(20);

            #region RelationOne-Many Currency-ExchangeRate
            
            modelBuilder.Entity<Currency>(entity =>
            {
                entity.Property(p => p.Symbol).IsRequired().HasMaxLength(10);
                entity.HasMany(e => e.BaseExchangeRates)
                  .WithOne(e => e.BaseCurrency)
                  .HasForeignKey(e => e.BaseCurrencyId)
                  .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.TargetExchangeRates)
                  .WithOne(e => e.TargetCurrency)
                  .HasForeignKey(e => e.TargetCurrencyId)
                  .OnDelete(DeleteBehavior.Restrict);
            });
            #endregion RelationOne-Many Currency-ExchangeRate
            modelBuilder.Entity<ExchangeRate>().Property(p => p.Rate).HasColumnType("decimal(18,4)");
        }

        public DbSet<Currency> Currencies { get; set; }
        public DbSet<ExchangeRate> ExchangesRates { get; set; }


    }
}
