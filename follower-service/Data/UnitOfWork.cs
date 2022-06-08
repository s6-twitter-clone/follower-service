using follower_service.Interfaces;

namespace follower_service.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly DatabaseContext context;

    public IFollowerRepository Followers { get; }
    public IUserRepository Users { get; }


    public UnitOfWork(DatabaseContext context)
    {
        this.context = context;

        Followers = new FollowerRepository(context);
        Users = new UserRepository(context);
    }



    public int Commit()
    {
        return context.SaveChanges();
    }

    public void Dispose()
    {
        context.Dispose();
    }
}