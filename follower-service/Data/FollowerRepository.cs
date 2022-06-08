using follower_service.Interfaces;
using follower_service.Models;

namespace follower_service.Data;

public class FollowerRepository : GenericRepository<Follower>, IFollowerRepository
{
    public FollowerRepository(DatabaseContext context) : base(context)
    {
    }

    public void AddFollower(User following, User followed)
    {
        var follower = new Follower
        {
            FollowingUser = following,
            FollowedUser = followed
        };

        _context.Followers.Add(follower);
    }

    public Follower? GetFollowerById(string followingId, string followedId)
    {
        return _context.Followers.FirstOrDefault(f => f.FollowingUserId == followingId && f.FollowedUserId == followedId);
    }
}