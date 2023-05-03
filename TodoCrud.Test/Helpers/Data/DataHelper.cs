namespace TodoCrud.Test.Helpers.Data;

using Microsoft.EntityFrameworkCore;
using Moq;

public static class DataHelper
{
    public static DbSet<T> GetQueryableMockDbSet<T>(List<T> sourceList) where T : class
    {
        var sourceQueryable = sourceList.AsQueryable();
        var mockDbSet = new Mock<DbSet<T>>();
        mockDbSet.As<IAsyncEnumerable<T>>()
            .Setup(m => m.GetAsyncEnumerator(default))
            .Returns(new TestDbAsyncEnumerator<T>(sourceQueryable.GetEnumerator()));
        mockDbSet.As<IQueryable<T>>()
            .Setup(m => m.Provider)
            .Returns(new TestDbAsyncQueryProvider<T>(sourceQueryable.Provider));
        mockDbSet.As<IQueryable<T>>()
            .Setup(m => m.Expression)
            .Returns(sourceQueryable.Expression);
        mockDbSet.As<IQueryable<T>>()
            .Setup(m => m.ElementType)
            .Returns(sourceQueryable.ElementType);
        mockDbSet.As<IQueryable<T>>()
            .Setup(m => m.GetEnumerator())
            .Returns(sourceQueryable.GetEnumerator());
        mockDbSet
            .Setup(m => m.Add(It.IsAny<T>()))
            .Callback<T>((s) => sourceList.Add(s));
        mockDbSet
            .Setup(m => m.Remove(It.IsAny<T>()))
            .Callback<T>((s) => sourceList.Remove(s));

        return mockDbSet.Object;
    }
}
