using follower_service.Interfaces;
using follower_service.Models;

namespace follower_service.Data;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(DatabaseContext context) : base(context)
    {
    }
}
