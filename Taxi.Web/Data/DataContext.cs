using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Taxi.Web.Data.Entities;

namespace Taxi.Web.Data
{
    public class DataContext : IdentityDbContext<UserEntity>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<TaxiEntity> Taxis { get; set; }

        public DbSet<TripEntity> Trips { get; set; }

        public DbSet<TripDetailEntity> TripDetails { get; set; }

        public DbSet<UserGroupEntity> UserGroups { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TaxiEntity>()
                .HasIndex(t => t.Plaque)
                .IsUnique();
        }
    }
}
