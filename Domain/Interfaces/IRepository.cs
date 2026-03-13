namespace NewRentalCarManagerAPI.Domain.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    IQueryable<T> Query();
    Task AddAsync(T entity);
    void Update(T entity);
    void Remove(T entity);
}
