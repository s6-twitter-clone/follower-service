﻿namespace follower_service.Models.Events;

public class AddFollowerEvent
{
    public string FollowedUserId { get; set; } = "";

    public string FollowingUserId { get; set; } = "";
}
