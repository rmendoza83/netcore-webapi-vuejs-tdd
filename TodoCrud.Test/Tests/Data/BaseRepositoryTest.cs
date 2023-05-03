namespace TodoCrud.Test.Tests.Data;

using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Moq;
using TodoCrud.Data;
using TodoCrud.Data.Models;
using TodoCrud.Data.Repositories;
using TodoCrud.Test.Helpers.Data;
using Xunit;
using static System.Runtime.InteropServices.JavaScript.JSType;

public abstract class BaseRepositoryTest<T> where T : class, IEntity, new()
{
    protected abstract IBaseRepository<T> GetSystemUnderTest(DatabaseContext context);

    [Fact]
    public async void GetAll_ShouldReturnAllEntitiesAsync()
    {
        // Arrange
        var faker = new Fixture();
        var entities = faker.Build<T>()
            .CreateMany(10)
            .ToList();
        var mockDbContext = new Mock<DatabaseContext>();
        var mockDbSet = DataHelper.GetQueryableMockDbSet(entities);

        mockDbContext
            .Setup(x => x.Set<T>())
            .Returns(mockDbSet);

        var repository = GetSystemUnderTest(mockDbContext.Object);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        result.Should().BeEquivalentTo(entities);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCorrectEntity()
    {
        // Arrange
        var faker = new Fixture();
        var entityId = faker.Create<Guid>();
        var entity = faker.Build<T>()
            .With(x => x.Id, entityId)
            .Create();

        var mockDbContext = new Mock<DatabaseContext>();

        mockDbContext
            .Setup(x => x.Set<T>().FindAsync(entityId))
            .ReturnsAsync(entity);

        var repository = GetSystemUnderTest(mockDbContext.Object);

        // Act
        var result = await repository.GetByIdAsync(entityId);

        // Assert
        result.Should().BeEquivalentTo(entity);
    }

    [Fact]
    public async Task InsertAsync_ShouldAddNewEntity()
    {
        // Arrange
        var faker = new Fixture();
        var entityId = faker.Create<Guid>();
        var entity = faker.Build<T>()
            .With(x => x.Id, entityId)
            .Create();

        var mockDbContext = new Mock<DatabaseContext>();
        var mockDbSet = new Mock<DbSet<T>>();

        mockDbContext
            .Setup(x => x.Set<T>())
            .Returns(mockDbSet.Object);

        var repository = GetSystemUnderTest(mockDbContext.Object);

        // Act
        await repository.InsertAsync(entity);

        // Assert
        mockDbContext.Verify(
            x => x.Set<T>().AddAsync(entity, default),
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
        var entity = faker.Build<T>()
            .With(x => x.Id, entityId)
            .Create();

        var mockDbContext = new Mock<DatabaseContext>();
        mockDbContext.Setup(x => x.SetModified(entity));

        var repository = GetSystemUnderTest(mockDbContext.Object);

        // Act
        await repository.UpdateAsync(entity);

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
        var entity = faker.Build<T>()
            .With(x => x.Id, entityId)
            .Create();

        var mockDbContext = new Mock<DatabaseContext>();

        mockDbContext
            .Setup(x => x.Set<T>().FindAsync(entityId))
            .ReturnsAsync(entity);

        var repository = GetSystemUnderTest(mockDbContext.Object);

        // Act
        await repository.DeleteAsync(entityId);

        // Assert
        mockDbContext.Verify(
            x => x.Set<T>().Remove(entity),
            Times.Once());
        mockDbContext.Verify(
            x => x.SaveChangesAsync(default),
            Times.Once());
    }
}

