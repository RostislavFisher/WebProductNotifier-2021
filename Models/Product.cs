using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebProductNotifier.Classes;
using WebProductNotifier.Models;

namespace WebProductNotifier.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string ItemID { get; set; }
        public string ShopKey { get; set; }
        
        public double Price { get; set; }

    }
    
    public class ProductDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();
                var connectionString = "Data Source=app.db";
                optionsBuilder.UseSqlite(connectionString);
            }
        }
        public DbSet<Product> Product { get; set; }
        
    }
}

