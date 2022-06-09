namespace follower_service.Models.Events;

public class DeleteFollowerEvent
{
    public string FollowedUserId { get; set; } = "";

    public string FollowingUserId { get; set; } = "";
}
