namespace Neusta.Jira.Connector.Infrastructure.AppPersistence.Interfaces;

public interface IAppPersistenceDbContext
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
