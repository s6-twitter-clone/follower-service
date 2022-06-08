using follower_service.Models;

namespace follower_service.Interfaces;

public interface IFollowerRepository : IGenericRepository<Follower>
{
    Follower? GetFollowerById(string followingId, string followedId);

    void AddFollower(User following, User followed);
}
