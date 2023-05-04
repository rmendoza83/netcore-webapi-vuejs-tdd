namespace TodoCrud.Test.Tests.Business;

using AutoFixture;
using FluentAssertions;
using Moq;
using TodoCrud.Business.Services;
using TodoCrud.Data.Models;
using TodoCrud.Data.Repositories;
using Xunit;

public abstract class BaseServiceTest<TEntity>
    where TEntity : class, IEntity, new()
{
    protected abstract BaseService<TEntity> GetSystemUnderTest();

    [Fact]
    public async Task GetAllAsync_ReturnsAllEntities()
    {
        // Arrange
        var faker = new Fixture();
        var entities = faker.Build<TEntity>()
            .CreateMany(10)
            .ToList();
        var mockRepository = new Mock<IBaseRepository<TEntity>>();
        //var mockRepository = new Mock<TRepository>();

        mockRepository
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(entities);

        var sut = GetSystemUnderTest();
        sut.Repository = mockRepository.Object;

        // Act
        var result = await sut.GetAllAsync();

        // Assert
        result.Should().BeEquivalentTo(entities);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingEntity_ReturnsEntity()
    {
        // Arrange
        var faker = new Fixture();
        var entityId = faker.Create<Guid>();
        var entity = faker.Build<TEntity>()
            .With(x => x.Id, entityId)
            .Create();

        var mockRepository = new Mock<IBaseRepository<TEntity>>();

        mockRepository
            .Setup(x => x.GetByIdAsync(entityId))
            .ReturnsAsync(entity);

        var sut = GetSystemUnderTest();
        sut.Repository = mockRepository.Object;

        // Act
        var result = await sut.GetByIdAsync(entityId);

        // Assert
        result.Should().BeEquivalentTo(entity);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingEntity_ReturnsNull()
    {
        // Arrange
        var faker = new Fixture();
        var entityId = faker.Create<Guid>();

        var mockRepository = new Mock<IBaseRepository<TEntity>>();

        mockRepository
            .Setup(x => x.GetByIdAsync(entityId))
            .Returns(Task.FromResult<TEntity?>(null));

        var sut = GetSystemUnderTest();
        sut.Repository = mockRepository.Object;

        // Act
        var result = await sut.GetByIdAsync(entityId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task InsertAsync_InsertsEntity()
    {
        // Arrange
        var faker = new Fixture();
        var entityId = faker.Create<Guid>();
        var entity = faker.Build<TEntity>()
            .With(x => x.Id, entityId)
            .Create();

        var mockRepository = new Mock<IBaseRepository<TEntity>>();

        var sut = GetSystemUnderTest();
        sut.Repository = mockRepository.Object;

        // Act
        await sut.InsertAsync(entity);

        // Assert
        mockRepository.Verify(
            x => x.InsertAsync(entity),
            Times.Once());
    }

    [Fact]
    public async Task UpdateAsync_UpdatesEntity()
    {
        // Arrange
        var faker = new Fixture();
        var entityId = faker.Create<Guid>();
        var entity = faker.Build<TEntity>()
            .With(x => x.Id, entityId)
            .Create();

        var mockRepository = new Mock<IBaseRepository<TEntity>>();

        var sut = GetSystemUnderTest();
        sut.Repository = mockRepository.Object;

        // Act
        await sut.UpdateAsync(entity);

        // Assert
        mockRepository.Verify(
            x => x.UpdateAsync(entity),
            Times.Once());
    }

    [Fact]
    public async Task DeleteAsync_DeletesEntity()
    {
        // Arrange
        var faker = new Fixture();
        var entityId = faker.Create<Guid>();

        var mockRepository = new Mock<IBaseRepository<TEntity>>();

        var sut = GetSystemUnderTest();
        sut.Repository = mockRepository.Object;

        // Act
        await sut.DeleteAsync(entityId);

        // Assert
        mockRepository.Verify(
            x => x.DeleteAsync(entityId),
            Times.Once());
    }
}
