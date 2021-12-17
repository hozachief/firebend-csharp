using System;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using System.IO;

#nullable disable

namespace Data
{
    public partial class AdventureWorks2019Context : DbContext
    {
        private DbContextOptions<AdventureWorks2019Context> options;
        private readonly DbConnection _connection;
        private AdventureWorks2019Context testContext;
        public AdventureWorks2019Context()
        {
        }

        public AdventureWorks2019Context(DbContextOptions<AdventureWorks2019Context> options)
            : base(options)
        {
            
        }

        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductCategory> ProductCategories { get; set; }
   public virtual DbSet<ProductDescription> ProductDescriptions { get; set; }
        public virtual DbSet<ProductInventory> ProductInventories { get; set; }
        public virtual DbSet<SalesOrderDetail> SalesOrderDetails { get; set; }

        // Unable to generate entity type for table 'Production.Document' since its primary key could not be scaffolded. Please see the warning messages.
        // Unable to generate entity type for table 'Production.ProductDocument' since its primary key could not be scaffolded. Please see the warning messages.

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

                //var connectionString = configuration.GetConnectionString("Data Source=./SeedData/testing.db;");

                optionsBuilder.UseSqlite("Data Source=./SeedData/testing.db;");
                //optionsBuilder.UseSqlServer(connectionString);
            }
        }
        private void BuildOptions()
        {
            options =  new DbContextOptionsBuilder<AdventureWorks2019Context>()
                .UseSqlite(CreateInMemoryDatabase())
                .Options;
        }
        private DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            
            
            return connection;
        }

        public AdventureWorks2019Context GetContext()
        {
            return testContext;
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Location>(entity =>
            {
                entity.ToTable("Location");

                entity.HasComment("Product inventory and manufacturing locations.");

                entity.HasIndex(e => e.Name, "AK_Location_Name")
                    .IsUnique();

                entity.Property(e => e.LocationId)
                    .HasColumnName("LocationID")
                    .HasComment("Primary key for Location records.");

                entity.Property(e => e.Availability)
                    .HasColumnType("decimal(8, 2)")
                    .HasDefaultValueSql("((0.00))")
                    .HasComment("Work capacity (in hours) of the manufacturing location.");

                entity.Property(e => e.CostRate)
                    .HasColumnType("smallmoney")
                    .HasDefaultValueSql("((0.00))")
                    .HasComment("Standard hourly cost of the manufacturing location.");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(datetime('now','localtime'))")
                    .HasComment("Date and time the record was last updated.");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasComment("Location description.");
            });

            modelBuilder.Entity<ProductInventory>(entity =>
            {
                entity.HasKey(e => new { e.ProductId, e.LocationId })
                    .HasName("PK_ProductInventory_ProductID_LocationID");

                entity.ToTable("ProductInventory");

                entity.HasComment("Product inventory information.");

                entity.Property(e => e.ProductId)
                    .HasColumnName("ProductID")
                    .HasComment("Product identification number. Foreign key to Product.ProductID.");

                entity.Property(e => e.LocationId)
                    .HasColumnName("LocationID")
                    .HasComment("Inventory location identification number. Foreign key to Location.LocationID. ");

                entity.Property(e => e.Bin).HasComment("Storage container on a shelf in an inventory location.");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(datetime('now','localtime'))")
                    .HasComment("Date and time the record was last updated.");

                entity.Property(e => e.Quantity).HasComment("Quantity of products in the inventory location.");

                entity.Property(e => e.Rowguid)
                    .HasColumnName("rowguid")
                    .HasComment("ROWGUIDCOL number uniquely identifying the record. Used to support a merge replication sample.");

                entity.Property(e => e.Shelf)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasComment("Storage compartment within an inventory location.");

                entity.HasOne(d => d.Location)
                    .WithMany(p => p.ProductInventories)
                    .HasForeignKey(d => d.LocationId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductInventories)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

  







            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
