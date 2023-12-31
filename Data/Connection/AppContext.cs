﻿using DromAutoTrader.Prices;
using SQLite;

namespace DromAutoTrader.Data.Connection
{
    public class AppContext : DbContext
    {
        //public AppContext(DbContextOptions<AppContext> options) : base(options)
        //{
        //}

        public DbSet<Supplier> Suppliers { get; set; } = null!;
        public DbSet<Brand> Brands { get; set; } = null!;
        public DbSet<Channel> Channels { get; set; } = null!;
        public DbSet<TablePriceOfIncrease> TablePriceOfIncreases { get; set; } = null!;
        public DbSet<PublishedPrice> PublishedPrices { get; set; } = null!;
        public DbSet<ImageService> ImageServices { get; set; } = null!;
        public DbSet<BrandImageServiceMapping> BrandImageServiceMappings { get; set; } = null!;
        public DbSet<BrandChannelMapping> BrandChannelMappings { get; set; } = null!;
        public DbSet<AdPublishingInfo> AdPublishingInfo { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=main.db");           
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            //// Определение таблицы для AdPublishingInfo
            //modelBuilder.Entity<AdPublishingInfo>()
            //    .ToTable("AdPublishingInfo"); // Указываем явное имя таблицы

            //modelBuilder.Entity<BrandChannelMapping>().HasNoKey();
            //// Добавьте здесь другие настройки и связи, если необходимо
        }
    }
}
