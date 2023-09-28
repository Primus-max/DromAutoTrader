using DromAutoTrader.Models;
using Microsoft.EntityFrameworkCore;

namespace DromAutoTrader.Data.Connection
{
    internal class AppContext : DbContext
    {
        public DbSet<Supplier> Suppliers { get; set; } = null!;
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=main.db");
        }
    }
}
