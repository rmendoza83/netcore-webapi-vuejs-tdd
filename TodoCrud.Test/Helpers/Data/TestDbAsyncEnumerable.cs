namespace TodoCrud.Test.Helpers.Data;

using Microsoft.EntityFrameworkCore.Query;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

public class TestDbAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
{
    public TestDbAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable) { }
    public TestDbAsyncEnumerable(Expression expression) : base(expression) { }

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return new TestDbAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
    }

    IQueryProvider IQueryable.Provider => new TestDbAsyncQueryProvider<T>(this);
}

public class TestDbAsyncEnumerator<T> : IAsyncEnumerator<T>
{
    private readonly IEnumerator<T> _inner;
    public TestDbAsyncEnumerator(IEnumerator<T> inner) => _inner = inner;
    public T Current => _inner.Current;
    public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(_inner.MoveNext());
    public ValueTask DisposeAsync() => new ValueTask(Task.CompletedTask);
}

public class TestDbAsyncQueryProvider<TEntity> : IAsyncQueryProvider
{
    private readonly IQueryProvider _inner;
    public TestDbAsyncQueryProvider(IQueryProvider inner) => _inner = inner;

    public IQueryable CreateQuery(Expression expression)
    {
        return new TestDbAsyncEnumerable<TEntity>(expression);
    }

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
    {
        return new TestDbAsyncEnumerable<TElement>(expression);
    }

    public object Execute(Expression expression)
    {
        return _inner.Execute(expression)!;
    }

    public TResult Execute<TResult>(Expression expression)
    {
        return _inner.Execute<TResult>(expression);
    }

    public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
    {
        var resultType = typeof(TResult).GetGenericArguments()[0];
        var executeMethod = _inner.GetType().GetMethod(nameof(IAsyncQueryProvider.ExecuteAsync), new[] { typeof(Expression), typeof(CancellationToken) });
        var executeGenericMethod = executeMethod!.MakeGenericMethod(resultType);
        return (TResult)executeGenericMethod.Invoke(_inner, new object[] { expression, cancellationToken })!;
    }
}