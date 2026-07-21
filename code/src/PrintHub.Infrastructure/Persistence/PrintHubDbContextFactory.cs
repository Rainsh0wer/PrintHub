using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PrintHub.Infrastructure.Persistence;

/// <summary>
/// Design-time factory used by the EF Core tools (`dotnet ef migrations add`,
/// `database update`) so migrations can be generated with Infrastructure as its
/// own startup project, independently of the API host. The runtime connection
/// string is supplied separately through configuration in the API.
/// </summary>
public class PrintHubDbContextFactory : IDesignTimeDbContextFactory<PrintHubDbContext>
{
    public PrintHubDbContext CreateDbContext(string[] args)
    {
        const string connectionString =
            @"Server=(localdb)\MSSQLLocalDB;Database=PrintHubDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

        var options = new DbContextOptionsBuilder<PrintHubDbContext>()
            .UseSqlServer(connectionString, sql =>
                sql.MigrationsAssembly(typeof(PrintHubDbContext).Assembly.FullName))
            .Options;

        return new PrintHubDbContext(options);
    }
}
