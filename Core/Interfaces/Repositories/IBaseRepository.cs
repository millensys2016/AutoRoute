
using System.Data;

namespace Core.Interfaces.Repositories
{
    public interface IBaseRepository<T> where T : class
    {
        Task<object?> GetAll(uint page = 1, uint limit = 10);
        Task<T?> GetById(uint id, string[]? includes = null);
        Task<T?> Add(T entity);
        Task<T?> Update(T entity);
        Task<T?> Delete(T entity);
    }
}
