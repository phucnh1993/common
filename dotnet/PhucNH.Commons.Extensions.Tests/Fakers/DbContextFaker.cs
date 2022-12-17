using Microsoft.EntityFrameworkCore;
using PhucNH.Commons.Bases.Models;

namespace PhucNH.Commons.Extensions.Tests.Fakers
{
    public class DbContextFaker : DbContext
    {
        public DbSet<BaseItem<ulong>> BaseItems { get; set; } = null!;

        public DbContextFaker(DbContextOptions<DbContextFaker> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BaseItem<ulong>>()
                .HasKey(x => new { x.Id });
        }
    }
}