using follower_service.Exceptions;
using follower_service.Interfaces;
using follower_service.Models;
using follower_service.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace follower_service_tests.Services;

public class UserServiceTests
{
    private readonly Mock<IUnitOfWork> unitOfWork = new();

    public UserServiceTests()
    {
        var user = new User();
        unitOfWork.Setup(x => x.Users.Add(user)).Returns(user);

        unitOfWork.Setup(x => x.Commit()).Returns(0);
    }

    [Fact]
    public void GetById_Success()
    {
        // Arrange
        var userService = new UserService(unitOfWork.Object);

        var user = new User
        {
            Id = "test-id",
            DisplayName = "name",
        };

        unitOfWork.Setup(x => x.Users.GetById(user.Id)).Returns(user);

        // Act
        var result = userService.GetById(user.Id);

        // Assert
        unitOfWork.Verify(x => x.Users.GetById(user.Id), Times.Once);

        Assert.Equal(user, result);
    }

    [Fact]
    public void GetById_NotFound()
    {
        // Arrange
        var userService = new UserService(unitOfWork.Object);

        var userId = "test-id";

        unitOfWork.Setup(x => x.Users.GetById(userId)).Returns(null as User);

        // Act
        var result = Assert.Throws<NotFoundException>(() =>
            userService.GetById(userId)
        );

        // Assert
        unitOfWork.Verify(x => x.Users.GetById(userId), Times.Once);

        Assert.Equal($"User with id '{userId}' doesn't exist.", result.Message);
    }


    [Fact]
    public void Add_Success()
    {
        // Arrange
        var userService = new UserService(unitOfWork.Object);

        var user = new User
        {
            Id = "test-id",
            DisplayName = "name",
        };

        unitOfWork.Setup(x => x.Commit()).Returns(1);

        // Act
        var result = userService.Add(user.Id, user.DisplayName);

        // Assert
        unitOfWork.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public void Add_CouldNotAdd()
    {
        // Arrange
        var userService = new UserService(unitOfWork.Object);

        var user = new User
        {
            Id = "test-id",
            DisplayName = "name",
        };

        // Act
        var result = Assert.Throws<InternalServerErrorException>(() =>
            userService.Add(user.Id, user.DisplayName)
        );

        // Assert
        unitOfWork.Verify(x => x.Commit(), Times.Once);

        Assert.Equal($"User with id '{user.Id}' could not be added.", result.Message);
    }

    [Fact]
    public void Update_Success()
    {
        // Arrange
        var userService = new UserService(unitOfWork.Object);

        var user = new User
        {
            Id = "test-id",
            DisplayName = "name",
        };

        var newUser = new User
        {
            Id = "test-id",
            DisplayName = "new name"
        };

        unitOfWork.Setup(x => x.Commit()).Returns(1);
        unitOfWork.Setup(x => x.Users.GetById(user.Id)).Returns(user);

        // Act
        var result = userService.Update(newUser.Id, newUser.DisplayName);

        // Assert
        unitOfWork.Verify(x => x.Users.GetById(user.Id), Times.Once);
        unitOfWork.Verify(x => x.Commit(), Times.Once);

        Assert.Equal(newUser.DisplayName, result.DisplayName);
    }

    [Fact]
    public void Update_NotFound()
    {
        // Arrange
        var userService = new UserService(unitOfWork.Object);

        var newUser = new User
        {
            Id = "test-id",
            DisplayName = "name",
        };

        unitOfWork.Setup(x => x.Commit()).Returns(1);
        unitOfWork.Setup(x => x.Users.GetById(newUser.Id)).Returns(null as User);

        // Act
        var result = Assert.Throws<NotFoundException>(() =>
            userService.Update(newUser.Id, newUser.DisplayName)
        );

        // Assert
        unitOfWork.Verify(x => x.Users.GetById(newUser.Id), Times.Once);
        unitOfWork.Verify(x => x.Commit(), Times.Never);

        Assert.Equal($"User with id '{newUser.Id}' doesn't exist.", result.Message);
    }

    [Fact]
    public void Update_CouldNotUpdate()
    {
        // Arrange
        var userService = new UserService(unitOfWork.Object);

        var user = new User
        {
            Id = "test-id",
            DisplayName = "name",
        };

        var newUser = new User
        {
            Id = "test-id",
            DisplayName = "new name"
        };

        unitOfWork.Setup(x => x.Users.GetById(user.Id)).Returns(user);

        // Act
        var result = Assert.Throws<InternalServerErrorException>(() =>
            userService.Update(newUser.Id, newUser.DisplayName)
        );

        // Assert
        unitOfWork.Verify(x => x.Users.GetById(user.Id), Times.Once);
        unitOfWork.Verify(x => x.Commit(), Times.Once);

        Assert.Equal($"User with id '{newUser.Id}' could not be updated.", result.Message);
    }
}
