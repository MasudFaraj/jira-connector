namespace Neusta.UserService.TestBase.Helper;

using System.Linq;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;

public static class MockHelper
{
    public static DbSet<T> CreateDbSet<T>(params T[] items)
        where T : class
    {
        Mock<DbSet<T>> mockedDbSet = items
            .AsQueryable()
            .BuildMockDbSet();

        return mockedDbSet.Object;
    }

    public static IQueryable<T> CreateQueryable<T>(params T[] items)
        where T : class
    {
        Mock<IQueryable<T>> mockedQueryable = items
            .AsQueryable()
            .BuildMock();

        return mockedQueryable.Object;
    }
}