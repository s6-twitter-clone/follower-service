using follower_service.Exceptions;
using follower_service.Interfaces;
using follower_service.Models;
using follower_service.Models.Events;

namespace follower_service.Services;

public class FollowerService
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IEventService eventService;

    public FollowerService(IUnitOfWork unitOfWork, IEventService eventService)
    {
        this.unitOfWork = unitOfWork;
        this.eventService = eventService;
    }


    public Follower GetById(string followingId, string followedId)
    {
        if(followingId == followedId)
        {
            throw new BadRequestException($"Users can't follow themselves.");
        }
        
        var followingUser = unitOfWork.Users.GetById(followingId);

        if (followingUser is null)
        {
            throw new BadRequestException($"User with id '{followingId}' doesn't exist.");
        }


        var followedUser = unitOfWork.Users.GetById(followedId);

        if (followedUser is null)
        {
            throw new BadRequestException($"User with id '{followedId}' doesn't exist.");
        }


        var follower = unitOfWork.Followers.GetFollowerById(followingId, followedId);

        if(follower is null)
        {
            throw new NotFoundException($"User with id '{followedId}' has no follower with id '{followingId}'.");
        }

        return follower;
    }

    public IEnumerable<User> GetFollowersById(string id)
    {
        var user = unitOfWork.Users.GetById(id);

        if (user is null)
        {
            throw new BadRequestException($"User with id '{id}' doesn't exist.");
        }

        return user.Following.Select(f => f.FollowingUser);
    }

    public IEnumerable<User> GetFollowingById(string id)
    {
        var user = unitOfWork.Users.GetById(id);

        if (user is null)
        {
            throw new BadRequestException($"User with id '{id}' doesn't exist.");
        }

        return user.Followed.Select(f => f.FollowingUser);
    }


    public void AddFollower(string followingId, string followedId)
    {
        if (followingId == followedId)
        {
            throw new BadRequestException($"Users can't follow themselves.");
        }

        var follower = unitOfWork.Users.GetById(followingId);

        if (follower is null)
        {
            throw new BadRequestException($"User with id '{followingId}' doesn't exist.");
        }

        var followee = unitOfWork.Users.GetById(followedId);

        if (followee is null)
        {
            throw new BadRequestException($"User with id '{followedId}' doesn't exist.");
        }

        unitOfWork.Followers.AddFollower(follower, followee);

        eventService.Publish(exchange: "follower-exchange", topic: "follower-added", new AddFollowerEvent
        {
            FollowedUserId = followedId,
            FollowingUserId = followingId
        });

        unitOfWork.Commit();
    }

    public void removeFollower(string followingId, string followedId)
    {
        if (followingId == followedId)
        {
            throw new BadRequestException($"Users can't follow themselves.");
        }

        var follower = GetById(followingId, followedId);

        unitOfWork.Followers.Remove(follower);

        eventService.Publish(exchange: "follower-exchange", topic: "follower-deleted", new DeleteFollowerEvent
        {
            FollowedUserId = followedId,
            FollowingUserId = followingId
        });

        unitOfWork.Commit();
    }

}
