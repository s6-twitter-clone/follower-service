namespace follower_service.Interfaces;

public interface IUnitOfWork
{
    public IFollowerRepository Followers { get; }
    public IUserRepository Users { get; }
    public int Commit();
}
