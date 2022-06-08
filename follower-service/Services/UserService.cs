using follower_service.Exceptions;
using follower_service.Interfaces;
using follower_service.Models;

namespace follower_service.Services;


public class UserService
{
    private readonly IUnitOfWork unitOfWork;

    public UserService(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    User GetById(string id)
    {
        var user = unitOfWork.Users.GetById(id);

        if (user is null)
        {
            throw new NotFoundException($"User with id '{id}' doesn't exist.");
        }

        return user;
    }

    public User Add(string id, string displayName)
    {
        var user = new User
        {
            Id = id,
            DisplayName = displayName,
        };

        unitOfWork.Users.Add(user);

        if (unitOfWork.Commit() < 1)
        {
            throw new InternalServerErrorException($"User with id '{id}' could not be added.");
        }

        return user;
    }

    public User Update(string id, string displayName)
    {
        var user = GetById(id);

        user.DisplayName = displayName;

        if (unitOfWork.Commit() < 1)
        {
            throw new InternalServerErrorException($"User with id '{id}' could not be updated.");
        }

        return user;
    }
}
