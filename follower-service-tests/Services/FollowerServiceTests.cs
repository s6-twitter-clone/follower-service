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

public class FollowerServiceTests
{
    private readonly Mock<IUnitOfWork> unitOfWork = new();
    private readonly Mock<IEventService> eventService = new();

    public FollowerServiceTests()
    {
        var follower = new Follower();
        unitOfWork.Setup(x => x.Followers.Add(follower)).Returns(follower);

        unitOfWork.Setup(x => x.Commit()).Returns(0);
    }

    [Fact]
    public void AddFollower_Success()
    {
        var followerService = new FollowerService(unitOfWork.Object, eventService.Object);
        var followedUser = new User { Id = "followed" };
        var followingUser = new User { Id = "following" };

        unitOfWork.Setup(x => x.Users.GetById(followedUser.Id)).Returns(followedUser);
        unitOfWork.Setup(x => x.Users.GetById(followingUser.Id)).Returns(followingUser);

        followerService.AddFollower(followingUser.Id, followedUser.Id);

        unitOfWork.Verify(x => x.Users.GetById(followedUser.Id), Times.Once);
        unitOfWork.Verify(x => x.Users.GetById(followingUser.Id), Times.Once);
        unitOfWork.Verify(x => x.Commit(), Times.Once);

        Assert.Single(eventService.Invocations.Where(i => i.Method.Name == "Publish"));
    }

    [Fact]
    public void AddFollower_FollowingSelf()
    {
        var followerService = new FollowerService(unitOfWork.Object, eventService.Object);
        var followingUser = new User { Id = "following" };

        var result = Assert.Throws<BadRequestException>(() => followerService.AddFollower(followingUser.Id, followingUser.Id));


        unitOfWork.Verify(x => x.Users.GetById(followingUser.Id), Times.Never);
        unitOfWork.Verify(x => x.Commit(), Times.Never);

        Assert.Empty(eventService.Invocations.Where(i => i.Method.Name == "Publish"));
        Assert.Equal("Users can't follow themselves.", result.Message);
    }

    [Fact]
    public void AddFollower_FollowerNotFound()
    {
        var followerService = new FollowerService(unitOfWork.Object, eventService.Object);
        var followedUser = new User { Id = "followed" };
        var followingUserId = "following";

        unitOfWork.Setup(x => x.Users.GetById(followedUser.Id)).Returns(followedUser);

        var result = Assert.Throws<BadRequestException>(() => followerService.AddFollower(followingUserId, followedUser.Id));


        unitOfWork.Verify(x => x.Users.GetById(followingUserId), Times.Once);
        unitOfWork.Verify(x => x.Commit(), Times.Never);

        Assert.Empty(eventService.Invocations.Where(i => i.Method.Name == "Publish"));
        Assert.Equal($"User with id '{followingUserId}' doesn't exist.", result.Message);
    }

    [Fact]
    public void AddFollower_FolloweeNotFound()
    {
        var followerService = new FollowerService(unitOfWork.Object, eventService.Object);
        var followingUser = new User { Id = "following" };
        var followedUserId = "followed";

        unitOfWork.Setup(x => x.Users.GetById(followingUser.Id)).Returns(followingUser);

        var result = Assert.Throws<BadRequestException>(() => followerService.AddFollower(followingUser.Id, followedUserId));


        unitOfWork.Verify(x => x.Users.GetById(followedUserId), Times.Once);
        unitOfWork.Verify(x => x.Commit(), Times.Never);

        Assert.Empty(eventService.Invocations.Where(i => i.Method.Name == "Publish"));
        Assert.Equal($"User with id '{followedUserId}' doesn't exist.", result.Message);
    }

    [Fact]
    public void AddFollower_AlreadyFollowing()
    {
        var followerService = new FollowerService(unitOfWork.Object, eventService.Object);
        var followedUser = new User { Id = "followed" };
        var followingUser = new User { Id = "following" };
        var follower = new Follower { FollowedUserId = followedUser.Id, FollowingUserId = followingUser.Id };

        unitOfWork.Setup(x => x.Users.GetById(followedUser.Id)).Returns(followedUser);
        unitOfWork.Setup(x => x.Users.GetById(followingUser.Id)).Returns(followingUser);
        unitOfWork.Setup(X => X.Followers.GetFollowerById(followingUser.Id, followedUser.Id)).Returns(follower);


        var result = Assert.Throws<BadRequestException>(() => followerService.AddFollower(followingUser.Id, followedUser.Id));

        unitOfWork.Verify(x => x.Commit(), Times.Never);

        Assert.Empty(eventService.Invocations.Where(i => i.Method.Name == "Publish"));
        Assert.Equal($"User with id '{followingUser.Id}' is already following user with id '{followedUser.Id}'.", result.Message);
    }


    [Fact]
    public void GetFollower_Success()
    {
        var followerService = new FollowerService(unitOfWork.Object, eventService.Object);
        var followedUser = new User { Id = "followed" };
        var followingUser = new User { Id = "following" };
        var follower = new Follower { FollowedUserId = followedUser.Id, FollowingUserId = followingUser.Id };

        unitOfWork.Setup(x => x.Users.GetById(followedUser.Id)).Returns(followedUser);
        unitOfWork.Setup(x => x.Users.GetById(followingUser.Id)).Returns(followingUser);
        unitOfWork.Setup(X => X.Followers.GetFollowerById(followingUser.Id, followedUser.Id)).Returns(follower);

        var result = followerService.GetById(followingUser.Id, followedUser.Id);

        unitOfWork.Verify(x => x.Users.GetById(followedUser.Id), Times.Once);
        unitOfWork.Verify(x => x.Users.GetById(followingUser.Id), Times.Once);

        Assert.Equal(followedUser.Id, result.FollowedUserId);
        Assert.Equal(followingUser.Id, result.FollowingUserId);
    }

    [Fact]
    public void GetFollower_FollowerNotFound()
    {
        var followerService = new FollowerService(unitOfWork.Object, eventService.Object);
        var followedUser = new User { Id = "followed" };
        var followingUserId = "following";

        var follower = new Follower { FollowedUserId = followedUser.Id, FollowingUserId = followingUserId };

        unitOfWork.Setup(x => x.Users.GetById(followedUser.Id)).Returns(followedUser);
        unitOfWork.Setup(X => X.Followers.GetFollowerById(followingUserId, followedUser.Id)).Returns(follower);

        var result = Assert.Throws<BadRequestException>(() => followerService.GetById(followingUserId, followedUser.Id));

        unitOfWork.Verify(x => x.Users.GetById(followingUserId), Times.Once);
        Assert.Equal($"User with id '{followingUserId}' doesn't exist.", result.Message);
    }

    [Fact]
    public void GetFollower_FolloweeNotFound()
    {
        var followerService = new FollowerService(unitOfWork.Object, eventService.Object);
        var followedUserId = "followed";
        var followingUser = new User { Id = "following" };

        var follower = new Follower { FollowedUserId = followedUserId, FollowingUserId = followingUser.Id };

        unitOfWork.Setup(x => x.Users.GetById(followingUser.Id)).Returns(followingUser);
        unitOfWork.Setup(X => X.Followers.GetFollowerById(followingUser.Id, followedUserId)).Returns(follower);

        var result = Assert.Throws<BadRequestException>(() => followerService.GetById(followingUser.Id, followedUserId));

        unitOfWork.Verify(x => x.Users.GetById(followedUserId), Times.Once);
        Assert.Equal($"User with id '{followedUserId}' doesn't exist.", result.Message);
    }

    [Fact]
    public void GetFollower_NotFollowing()
    {
        var followerService = new FollowerService(unitOfWork.Object, eventService.Object);
        var followedUser = new User { Id = "followed" };
        var followingUser = new User { Id = "following" };

        unitOfWork.Setup(x => x.Users.GetById(followedUser.Id)).Returns(followedUser);
        unitOfWork.Setup(x => x.Users.GetById(followingUser.Id)).Returns(followingUser);
        unitOfWork.Setup(X => X.Followers.GetFollowerById(followingUser.Id, followedUser.Id)).Returns(null as Follower);

        var result = Assert.Throws<NotFoundException>(() => followerService.GetById(followingUser.Id, followedUser.Id));

        unitOfWork.Verify(x => x.Users.GetById(followedUser.Id), Times.Once);
        unitOfWork.Verify(x => x.Users.GetById(followingUser.Id), Times.Once);
        unitOfWork.Verify(x => x.Followers.GetFollowerById(followingUser.Id, followedUser.Id), Times.Once);

        Assert.Equal($"User with id '{followedUser.Id}' has no follower with id '{followingUser.Id}'.", result.Message);
    }

    [Fact]
    public void RemoveFollower_Success()
    {
        var followerService = new FollowerService(unitOfWork.Object, eventService.Object);
        var followedUser = new User { Id = "followed" };
        var followingUser = new User { Id = "following" };
        var follower = new Follower { FollowedUserId = followedUser.Id, FollowingUserId = followingUser.Id };

        unitOfWork.Setup(x => x.Users.GetById(followedUser.Id)).Returns(followedUser);
        unitOfWork.Setup(x => x.Users.GetById(followingUser.Id)).Returns(followingUser);
        unitOfWork.Setup(X => X.Followers.GetFollowerById(followingUser.Id, followedUser.Id)).Returns(follower);

        followerService.removeFollower(followingUser.Id, followedUser.Id);

        unitOfWork.Verify(x => x.Users.GetById(followedUser.Id), Times.Once);
        unitOfWork.Verify(x => x.Users.GetById(followingUser.Id), Times.Once);
        unitOfWork.Verify(x => x.Followers.GetFollowerById(followingUser.Id, followedUser.Id), Times.Once);
        unitOfWork.Verify(x => x.Commit(), Times.Once);

        Assert.Single(eventService.Invocations.Where(i => i.Method.Name == "Publish"));
    }

    [Fact]
    public void RemoveFollower_FollowingSelf()
    {
        var followerService = new FollowerService(unitOfWork.Object, eventService.Object);
        var followingUser = new User { Id = "following" };

        unitOfWork.Setup(x => x.Users.GetById(followingUser.Id)).Returns(followingUser);

        var result = Assert.Throws<BadRequestException>(() => followerService.removeFollower(followingUser.Id, followingUser.Id));


        unitOfWork.Verify(x => x.Commit(), Times.Never);

        Assert.Empty(eventService.Invocations.Where(i => i.Method.Name == "Publish"));
        Assert.Equal("Users can't follow themselves.", result.Message);
    }


    [Fact]
    public void RemoveFollower_NotFollowing()
    {
        var followerService = new FollowerService(unitOfWork.Object, eventService.Object);
        var followedUser = new User { Id = "followed" };
        var followingUser = new User { Id = "following" };

        unitOfWork.Setup(x => x.Users.GetById(followedUser.Id)).Returns(followedUser);
        unitOfWork.Setup(x => x.Users.GetById(followingUser.Id)).Returns(followingUser);
        unitOfWork.Setup(X => X.Followers.GetFollowerById(followingUser.Id, followedUser.Id)).Returns(null as Follower);

        var result = Assert.Throws<NotFoundException>(() => followerService.removeFollower(followingUser.Id, followedUser.Id));

        unitOfWork.Verify(x => x.Users.GetById(followedUser.Id), Times.Once);
        unitOfWork.Verify(x => x.Users.GetById(followingUser.Id), Times.Once);
        unitOfWork.Verify(x => x.Followers.GetFollowerById(followingUser.Id, followedUser.Id), Times.Once);
        unitOfWork.Verify(x => x.Commit(), Times.Never);

        Assert.Empty(eventService.Invocations.Where(i => i.Method.Name == "Publish"));
        Assert.Equal($"User with id '{followedUser.Id}' has no follower with id '{followingUser.Id}'.", result.Message);
    }

    [Fact]
    public void RemoveFollower_FollowerNotFound()
    {
        var followerService = new FollowerService(unitOfWork.Object, eventService.Object);
        var followedUser = new User { Id = "followed" };
        var followingUserId = "following";

        unitOfWork.Setup(x => x.Users.GetById(followedUser.Id)).Returns(followedUser);

        var result = Assert.Throws<BadRequestException>(() => followerService.removeFollower(followingUserId, followedUser.Id));
        unitOfWork.Verify(x => x.Users.GetById(followingUserId), Times.Once);
        unitOfWork.Verify(x => x.Commit(), Times.Never);

        Assert.Empty(eventService.Invocations.Where(i => i.Method.Name == "Publish"));
        Assert.Equal($"User with id '{followingUserId}' doesn't exist.", result.Message);
    }

    [Fact]
    public void RemoveFollower_FolloweeNotFound()
    {
        var followerService = new FollowerService(unitOfWork.Object, eventService.Object);
        var followedUserId = "followed";
        var followingUser = new User { Id = "following" };

        unitOfWork.Setup(x => x.Users.GetById(followingUser.Id)).Returns(followingUser);

        var result = Assert.Throws<BadRequestException>(() => followerService.removeFollower(followingUser.Id, followedUserId));
        unitOfWork.Verify(x => x.Users.GetById(followedUserId), Times.Once);
        unitOfWork.Verify(x => x.Commit(), Times.Never);

        Assert.Empty(eventService.Invocations.Where(i => i.Method.Name == "Publish"));
        Assert.Equal($"User with id '{followedUserId}' doesn't exist.", result.Message);
    }

    
}
