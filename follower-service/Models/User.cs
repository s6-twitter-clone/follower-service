namespace follower_service.Models;

public class User
{
    public string Id { get; set; } = "";
    public string DisplayName { get; set; } = "";

    public virtual ICollection<Follower> Followed { get; set; } = new List<Follower>();
    public virtual ICollection<Follower> Following { get; set; } = new List<Follower>();
}
