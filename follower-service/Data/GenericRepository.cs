using follower_service.Interfaces;

namespace follower_service.Data;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly DatabaseContext _context;

    public GenericRepository(DatabaseContext context)
    {
        _context = context;
    }

    public T Add(T entity)
    {
        return _context.Set<T>().Add(entity).Entity;
    }

    public IEnumerable<T> GetAll()
    {
        return _context.Set<T>().ToList();
    }

    public T? GetById(string id)
    {
        return _context.Set<T>().Find(id);
    }

    public void Remove(T entity)
    {
        _context.Set<T>().Remove(entity);
    }
}