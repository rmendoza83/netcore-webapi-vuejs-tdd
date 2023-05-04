using AutoFixture;
using FluentAssertions;
using FluentAssertions.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TodoCrud.Api.Controllers;
using TodoCrud.Business.Services;
using TodoCrud.Data.Models;
using Xunit;

namespace TodoCrud.Test.Tests.Api;

public abstract class BaseControllerTest<TEntity>
    where TEntity : class, IEntity, new()
{
    protected abstract BaseController<TEntity> GetSystemUnderTest(IBaseService<TEntity> service, ILogger<IBaseService<TEntity>> logger);

    private BaseController<TEntity> PreGetSystemUnderTest(IBaseService<TEntity> service, ILogger<IBaseService<TEntity>> logger)
    {
        var defaultContext = new DefaultHttpContext();
        var controllerContext = new ControllerContext()
        {
            HttpContext = defaultContext
        };

        var controller = GetSystemUnderTest(service, logger);
        controller.ControllerContext = controllerContext;

        return controller;
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsEntity_WhenEntityFound()
    {
        // Arrange
        var faker = new Fixture();
        var entityId = faker.Create<Guid>();
        var entity = faker.Build<TEntity>()
            .With(x => x.Id, entityId)
            .Create();
        var mockService = new Mock<IBaseService<TEntity>>();
        var mockLogger = new Mock<ILogger<IBaseService<TEntity>>>();

        mockService
            .Setup(x => x.GetByIdAsync(entityId))
            .ReturnsAsync(entity);

        var sut = PreGetSystemUnderTest(mockService.Object, mockLogger.Object);

        // Act
        var response = await sut.GetByIdAsync(entityId);

        // Assert
        response.Result.Should().BeOfType<OkObjectResult>();
        response.Result.As<OkObjectResult>().Value.Should().BeEquivalentTo(entity);        
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNotFound_WhenEntityNotFound()
    {
        // Arrange
        var faker = new Fixture();
        var entityId = faker.Create<Guid>();
        var mockService = new Mock<IBaseService<TEntity>>();
        var mockLogger = new Mock<ILogger<IBaseService<TEntity>>>();

        mockService
            .Setup(x => x.GetByIdAsync(entityId))
            .Returns(Task.FromResult<TEntity?>(null));

        var sut = PreGetSystemUnderTest(mockService.Object, mockLogger.Object);

        // Act
        var response = await sut.GetByIdAsync(entityId);

        // Assert
        response.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsInternalServerError_WhenAnExceptionIsThrow()
    {
        // Arrange
        var faker = new Fixture();
        var entityId = faker.Create<Guid>();
        var mockService = new Mock<IBaseService<TEntity>>();
        var mockLogger = new Mock<ILogger<IBaseService<TEntity>>>();

        mockService
            .Setup(x => x.GetByIdAsync(entityId))
            .ThrowsAsync(faker.Create<Exception>());

        var sut = PreGetSystemUnderTest(mockService.Object, mockLogger.Object);

        // Act
        var response = await sut.GetByIdAsync(entityId);

        // Assert
        response.Result.Should().BeOfType<StatusCodeResult>();
        response.Result.As<StatusCodeResult>().StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOkObjectResult_WithListOfEntities()
    {
        // Arrange
        var faker = new Fixture();
        var entities = faker.Build<TEntity>()
            .CreateMany(10)
            .ToList();
        var mockService = new Mock<IBaseService<TEntity>>();
        var mockLogger = new Mock<ILogger<IBaseService<TEntity>>>();

        mockService
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(entities);

        var sut = PreGetSystemUnderTest(mockService.Object, mockLogger.Object);

        // Act
        var response = await sut.GetAllAsync();

        // Assert
        response.Result.Should().BeOfType<OkObjectResult>();
        response.Result.As<OkObjectResult>().StatusCode.Should().Be(StatusCodes.Status200OK);
        response.Result.As<OkObjectResult>().Value.Should().BeOfType<List<TEntity>>();
        response.Result.As<OkObjectResult>().Value.As<List<TEntity>>().Count.Should().Be(entities.Count);
        response.Result.As<OkObjectResult>().Value.As<List<TEntity>>().Should().BeEquivalentTo(entities);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOkObjectResult_WithEmptyResponseBecauseAnExceptionIsThrow()
    {
        // Arrange
        var faker = new Fixture();
        var mockService = new Mock<IBaseService<TEntity>>();
        var mockLogger = new Mock<ILogger<IBaseService<TEntity>>>();

        mockService
            .Setup(x => x.GetAllAsync())
            .ThrowsAsync(faker.Create<Exception>());

        var sut = PreGetSystemUnderTest(mockService.Object, mockLogger.Object);

        // Act
        var response = await sut.GetAllAsync();

        // Assert
        response.Result.Should().BeOfType<OkResult>();
        response.Result.As<OkResult>().StatusCode.Should().Be(StatusCodes.Status200OK);
    }

    [Fact]
    public async Task InsertAsync_ShouldReturnCreatedResponse()
    {
        // Arrange
        var faker = new Fixture();
        var entity = faker.Create<TEntity>();
        var mockService = new Mock<IBaseService<TEntity>>();
        var mockLogger = new Mock<ILogger<IBaseService<TEntity>>>();

        mockService
            .Setup(x => x.InsertAsync(entity))
            .Verifiable();

        var sut = PreGetSystemUnderTest(mockService.Object, mockLogger.Object);

        // Act
        var response = await sut.InsertAsync(entity);

        // Assert
        response.Should().BeOfType<CreatedAtActionResult>();
        response.As<CreatedAtActionResult>().Value.Should().Be(entity);
        response.As<CreatedAtActionResult>().ActionName.Should().Be(nameof(sut.GetByIdAsync));
        response.As<CreatedAtActionResult>().RouteValues.Should().ContainKey("id").And.Subject["id"].Should().Be(entity.Id);
        mockService.Verify();
    }

    [Fact]
    public async Task InsertAsync_ReturnsInternalServerError_WhenAnExceptionIsThrow()
    {
        // Arrange
        var faker = new Fixture();
        var entity = faker.Create<TEntity>();
        var mockService = new Mock<IBaseService<TEntity>>();
        var mockLogger = new Mock<ILogger<IBaseService<TEntity>>>();

        mockService
            .Setup(x => x.InsertAsync(entity))
            .ThrowsAsync(faker.Create<Exception>());

        var sut = PreGetSystemUnderTest(mockService.Object, mockLogger.Object);

        // Act
        var response = await sut.InsertAsync(entity);

        // Assert
        response.Should().BeOfType<StatusCodeResult>();
        response.As<StatusCodeResult>().StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task UpdateAsync_WithValidIdAndTodoItem_ReturnsNoContent()
    {
        // Arrange
        var faker = new Fixture();
        var entityId = faker.Create<Guid>();
        var entity = faker.Build<TEntity>()
            .With(x => x.Id, entityId)
            .Create();
        var mockService = new Mock<IBaseService<TEntity>>();
        var mockLogger = new Mock<ILogger<IBaseService<TEntity>>>();

        mockService
            .Setup(x => x.UpdateAsync(entity))
            .Verifiable();

        var sut = PreGetSystemUnderTest(mockService.Object, mockLogger.Object);

        // Act
        var response = await sut.UpdateAsync(entityId, entity);

        // Assert
        response.Should().BeOfType<NoContentResult>();
        mockService.Verify();
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidIdAndTodoItem_ReturnsBadRequest()
    {
        // Arrange
        var faker = new Fixture();
        var entityId = faker.Create<Guid>();
        var entity = faker.Build<TEntity>()
            .With(x => x.Id, faker.Create<Guid>())
            .Create();
        var mockService = new Mock<IBaseService<TEntity>>();
        var mockLogger = new Mock<ILogger<IBaseService<TEntity>>>();

        var sut = PreGetSystemUnderTest(mockService.Object, mockLogger.Object);

        // Act
        var response = await sut.UpdateAsync(entityId, entity);

        // Assert
        response.Should().BeOfType<BadRequestResult>();
        mockService.Verify(
            x => x.UpdateAsync(entity),
            Times.Never()
        );
    }

    [Fact]
    public async Task UpdateAsync_ReturnsInternalServerError_WhenAnExceptionIsThrow()
    {
        // Arrange
        var faker = new Fixture();
        var entityId = faker.Create<Guid>();
        var entity = faker.Build<TEntity>()
            .With(x => x.Id, entityId)
            .Create();
        var mockService = new Mock<IBaseService<TEntity>>();
        var mockLogger = new Mock<ILogger<IBaseService<TEntity>>>();

        mockService
            .Setup(x => x.UpdateAsync(entity))
            .ThrowsAsync(faker.Create<Exception>());

        var sut = PreGetSystemUnderTest(mockService.Object, mockLogger.Object);

        // Act
        var response = await sut.UpdateAsync(entityId, entity);

        // Assert
        response.Should().BeOfType<StatusCodeResult>();
        response.As<StatusCodeResult>().StatusCode.Should().Be(500);
        mockService.Verify(
            x => x.UpdateAsync(entity),
            Times.Once()
        );
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_ShouldReturnNoContent()
    {
        // Arrange
        var faker = new Fixture();
        var entityId = faker.Create<Guid>();
        var mockService = new Mock<IBaseService<TEntity>>();
        var mockLogger = new Mock<ILogger<IBaseService<TEntity>>>();

        mockService
            .Setup(x => x.DeleteAsync(entityId))
            .Verifiable();

        var sut = PreGetSystemUnderTest(mockService.Object, mockLogger.Object);

        // Act
        var response = await sut.DeleteAsync(entityId);

        // Assert
        response.Should().BeOfType<NoContentResult>();
        mockService.Verify();
    }

    [Fact]
    public async Task DeleteAsync_ReturnsInternalServerError_WhenAnExceptionIsThrow()
    {
        // Arrange
        var faker = new Fixture();
        var entityId = faker.Create<Guid>();
        var mockService = new Mock<IBaseService<TEntity>>();
        var mockLogger = new Mock<ILogger<IBaseService<TEntity>>>();

        mockService
            .Setup(x => x.DeleteAsync(entityId))
            .ThrowsAsync(faker.Create<Exception>());

        var sut = PreGetSystemUnderTest(mockService.Object, mockLogger.Object);

        // Act
        var response = await sut.DeleteAsync(entityId);

        // Assert
        response.Should().BeOfType<StatusCodeResult>();
        response.As<StatusCodeResult>().StatusCode.Should().Be(500);
        mockService.Verify(
            x => x.DeleteAsync(entityId),
            Times.Once()
        );
    }
}
