// PaymentService/PaymentDbContextFactory.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace PaymentService.Data;

public class PaymentDbContextFactory : IDesignTimeDbContextFactory<PaymentDbContext>
{
    public PaymentDbContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        // ✅ نام صحیح: PaymentConnection
        var connStr = config.GetConnectionString("PaymentConnection");

        if (string.IsNullOrEmpty(connStr))
        {
            throw new InvalidOperationException("Connection string 'PaymentConnection' not found.");
        }

        var options = new DbContextOptionsBuilder<PaymentDbContext>()
            .UseSqlServer(connStr)
            .Options;

        return new PaymentDbContext(options);
    }
}