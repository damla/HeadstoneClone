using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Models
{
    public class HeadstoneDbContext : DbContext
    {
        public HeadstoneDbContext() : base("name=MAIN")
        {

        }

        public static HeadstoneDbContext Create()
        {
            return new HeadstoneDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<HeadstoneDbContext>(null);
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<TaxOffice> TaxOffices { get; set; }

        public DbSet<GeoLocation> GeoLocations { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderLine> OrderLines { get; set; }

        public DbSet<Basket> Baskets { get; set; }

        public DbSet<BasketItem> BasketItems { get; set; }

        public DbSet<Campaign> Campaigns { get; set; }

        public DbSet<Coupon> Coupon { get; set; }

        public DbSet<Transaction> Transactions { get; set; }

        public DbSet<FavoriteProducts> FavoriteProducts { get; set; }

    }
}
