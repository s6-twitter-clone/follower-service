namespace follower_service.Dtos;

public class FollowerDTO
{
    public UserDTO FollowedUser { get; set; } = new UserDTO();
    public UserDTO FollowingUser { get; set; } = new UserDTO();
}
