namespace Neusta.Jira.Connector.Infrastructure.AppPersistence;

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Neusta.Jira.Connector.Infrastructure.AppPersistence.Entities;
using Neusta.Jira.Connector.Infrastructure.AppPersistence.Interfaces;

[ExcludeFromCodeCoverage]
public class AppPersistenceDbContext : DbContext, IAppPersistenceDbContext
{
    public AppPersistenceDbContext(DbContextOptions<AppPersistenceDbContext> options)
        : base(options)
    {   
    }
    public DbSet<UserEntity> Users => this.Set<UserEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
