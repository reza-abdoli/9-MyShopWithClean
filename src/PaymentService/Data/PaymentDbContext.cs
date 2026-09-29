// PaymentService/Data/PaymentDbContext.cs
using Microsoft.EntityFrameworkCore;

namespace PaymentService.Data;

public class PaymentDbContext : DbContext
{
    public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options) { }
    public DbSet<PaymentTransaction> PaymentTransactions => Set<PaymentTransaction>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<PaymentTransaction>().HasKey(t => t.PaymentId);
    }
}