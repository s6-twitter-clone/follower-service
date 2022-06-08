namespace follower_service.Models;

public class Follower
{
    public string Id { get; set; } = "";

    public virtual User FollowedUser { get; set; } = new User();
    public string FollowedUserId { get; set; } = "";

    public virtual User FollowingUser { get; set; } = new User();
    public string FollowingUserId { get; set; } = "";
}
