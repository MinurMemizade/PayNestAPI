using PayNestAPI.Context;
using PayNestAPI.Models.Common;

namespace PayNestAPI.Repositories.Interfaces
{
    public interface IRepository<T> where T : BaseEntity, new()
    {
        Task<List<T>> GetAllAsync();
        Task<T> GetAsync(Guid id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(Guid id);
    }
}
