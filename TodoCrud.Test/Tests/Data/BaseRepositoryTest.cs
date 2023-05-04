namespace TodoCrud.Test.Tests.Data;

using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using TodoCrud.Data;
using TodoCrud.Data.Models;
using TodoCrud.Data.Repositories;
using TodoCrud.Test.Helpers.Data;
using Xunit;

public abstract class BaseRepositoryTest<TEntity> where TEntity : class, IEntity, new()
{
    protected abstract IBaseRepository<TEntity> GetSystemUnderTest(DatabaseContext context);

    [Fact]
    public async void GetAll_ShouldReturnAllEntitiesAsync()
    {
        // Arrange
        var faker = new Fixture();
        var entities = faker.Build<TEntity>()
            .CreateMany(10)
            .ToList();
        var mockDbContext = new Mock<DatabaseContext>();
        var mockDbSet = DataHelper.GetQueryableMockDbSet(entities);

        mockDbContext
            .Setup(x => x.Set<TEntity>())
            .Returns(mockDbSet);

        var sut = GetSystemUnderTest(mockDbContext.Object);

        // Act
        var result = await sut.GetAllAsync();

        // Assert
        result.Should().BeEquivalentTo(entities);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCorrectEntity()
    {
        // Arrange
        var faker = new Fixture();
        var entityId = faker.Create<Guid>();
        var entity = faker.Build<TEntity>()
            .With(x => x.Id, entityId)
            .Create();

        var mockDbContext = new Mock<DatabaseContext>();

        mockDbContext
            .Setup(x => x.Set<TEntity>().FindAsync(entityId))
            .ReturnsAsync(entity);

        var sut = GetSystemUnderTest(mockDbContext.Object);

        // Act
        var result = await sut.GetByIdAsync(entityId);

        // Assert
        result.Should().BeEquivalentTo(entity);
    }

    [Fact]
    public async Task InsertAsync_ShouldAddNewEntity()
    {
        // Arrange
        var faker = new Fixture();
        var entityId = faker.Create<Guid>();
        var entity = faker.Build<TEntity>()
            .With(x => x.Id, entityId)
            .Create();

        var mockDbContext = new Mock<DatabaseContext>();
        var mockDbSet = new Mock<DbSet<TEntity>>();

        mockDbContext
            .Setup(x => x.Set<TEntity>())
            .Returns(mockDbSet.Object);

        var sut = GetSystemUnderTest(mockDbContext.Object);

        // Act
        await sut.InsertAsync(entity);

        // Assert
        mockDbContext.Verify(
            x => x.Set<TEntity>().AddAsync(entity, default),
            Times.Once());
        mockDbContext.Verify(
            x => x.SaveChangesAsync(default),
            Times.Once());
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateExistingEntity()
    {
        // Arrange
        var faker = new Fixture();
        var entityId = faker.Create<Guid>();
        var entity = faker.Build<TEntity>()
            .With(x => x.Id, entityId)
            .Create();

        var mockDbContext = new Mock<DatabaseContext>();
        mockDbContext.Setup(x => x.SetModified(entity));

        var sut = GetSystemUnderTest(mockDbContext.Object);

        // Act
        await sut.UpdateAsync(entity);

        // Assert
        mockDbContext.Verify(
            x => x.SaveChangesAsync(default),
            Times.Once());
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteExistingEntity()
    {
        // Arrange
        var faker = new Fixture();
        var entityId = faker.Create<Guid>();
        var entity = faker.Build<TEntity>()
            .With(x => x.Id, entityId)
            .Create();

        var mockDbContext = new Mock<DatabaseContext>();

        mockDbContext
            .Setup(x => x.Set<TEntity>().FindAsync(entityId))
            .ReturnsAsync(entity);

        var sut = GetSystemUnderTest(mockDbContext.Object);

        // Act
        await sut.DeleteAsync(entityId);

        // Assert
        mockDbContext.Verify(
            x => x.Set<TEntity>().Remove(entity),
            Times.Once());
        mockDbContext.Verify(
            x => x.SaveChangesAsync(default),
            Times.Once());
    }
}

