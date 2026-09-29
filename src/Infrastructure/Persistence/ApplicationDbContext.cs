using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Payments;
using Domain.Entities;
using Domain.Entities.Common;
using Domain.Entities.Order;
using Domain.Entities.Product;
using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace Persistence;


public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Brand> Brand { get; set; }
    public DbSet<Product> Product { get; set; }
    public DbSet<ProductCategory> ProductCategory { get; set; }
    public DbSet<ProductColor> ProductColor { get; set; }
    public DbSet<ProductFeature> ProductFeature { get; set; }
    public DbSet<ProductGallery> ProductGallery { get; set; }
    public DbSet<ProductSelectedCategory> ProductSelectedCategory { get; set; }
    public DbSet<ProductSize> ProductSize { get; set; }
    public DbSet<ProductVariant> ProductVariant { get; set; }

    public DbSet<Banner> Banners { get; set; }
    public DbSet<Order> Order { get; set; }
    public DbSet<OrderDetail> OrderDetail { get; set; }
    public DbSet<ProductSell> ProductSell { get; set; }
    public DbSet<PaymentTransaction> PaymentTransaction { get; set; }
    public DbSet<OrderPaymentSaga> OrderPaymentSagas { get; set; }
    public DbSet<WalletPaymentAttempt> WalletPaymentAttempts { get; set; }
    public DbSet<OnlineOrderPaymentSaga> OnlineOrderPaymentSagas { get; set; }
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        var cascadeFKs = modelBuilder.Model.GetEntityTypes()
            .SelectMany(t => t.GetForeignKeys())
            .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

        foreach (var fk in cascadeFKs)
            fk.DeleteBehavior = DeleteBehavior.Restrict;

        modelBuilder.Entity<Product>()
            .HasOne(p => p.Brand)
            .WithMany(b => b.Products)
            .HasForeignKey(p => p.BrandId)
            .IsRequired(false);

        modelBuilder.Entity<PaymentTransaction>()
            .HasOne(pt => pt.Order)
            .WithOne(o => o.PaymentTransaction)
            .HasForeignKey<PaymentTransaction>(pt => pt.OrderId)
            .IsRequired(false);

        modelBuilder.Entity<ProductVariant>()
            .Property(x => x.RowVersion)
            .IsRowVersion();
        modelBuilder.Entity<User>()
            .Property(x => x.RowVersion)
            .IsRowVersion();

        modelBuilder.Entity<OrderPaymentSaga>().HasKey(s => s.Id);

        modelBuilder.Entity<WalletPaymentAttempt>().HasIndex(a => a.PaymentId).IsUnique();

        modelBuilder.Entity<OnlineOrderPaymentSaga>().HasKey(s => s.Id);

        modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
        modelBuilder.Entity<Brand>().HasQueryFilter(b => !b.IsDeleted);
        modelBuilder.Entity<Product>().HasQueryFilter(p => !p.IsDeleted);
        modelBuilder.Entity<ProductCategory>().HasQueryFilter(pc => !pc.IsDeleted);
        modelBuilder.Entity<ProductColor>().HasQueryFilter(pc => !pc.IsDeleted);
        modelBuilder.Entity<ProductFeature>().HasQueryFilter(pf => !pf.IsDeleted);
        modelBuilder.Entity<ProductGallery>().HasQueryFilter(pg => !pg.IsDeleted);
        modelBuilder.Entity<ProductSelectedCategory>().HasQueryFilter(psc => !psc.IsDeleted);
        modelBuilder.Entity<ProductSize>().HasQueryFilter(ps => !ps.IsDeleted);
        modelBuilder.Entity<ProductVariant>().HasQueryFilter(pv => !pv.IsDeleted);
        modelBuilder.Entity<Banner>().HasQueryFilter(pv => !pv.IsDeleted);
        modelBuilder.Entity<Order>().HasQueryFilter(pv => !pv.IsDeleted);
        modelBuilder.Entity<OrderDetail>().HasQueryFilter(pv => !pv.IsDeleted);
        modelBuilder.Entity<ProductSell>().HasQueryFilter(pv => !pv.IsDeleted);
        modelBuilder.Entity<PaymentTransaction>().HasQueryFilter(pv => !pv.IsDeleted);
    }
}