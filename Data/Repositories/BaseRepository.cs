using Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class BaseRepository<T>(AppDbContext context) : IBaseRepository<T> where T : class
    {
        protected AppDbContext _context = context;

        public async Task<object?> GetAll(uint page = 1, uint limit = 10)
        {
            try
            {
                var query = _context.Set<T>();

                var totalItemsCount = await query.CountAsync();

                var items = await query
                    .Skip((int)((page - 1) * limit))
                    .Take((int)limit)
                    .ToListAsync();

                return new
                {
                    items,
                    totalItemsCount,
                    page,
                    limit
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }


        public async Task<T?> GetById(uint id, string[]? includes = null)
        {
            try
            {
                IQueryable<T> query = _context.Set<T>();

                if (includes != null && includes.Length > 0)
                {
                    foreach (var include in includes)
                    {
                        query.Include(include);
                    }
                }

                return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        public async Task<T?> Add(T entity)
        {
            try
            {
                await _context.Set<T>().AddAsync(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<T?> Update(T entity)
        {
            try
            {
                _context.Set<T>().Update(entity);

                var updatedAt = typeof(T).GetProperty("UpdatedAt");

                if (updatedAt != null && updatedAt.PropertyType == typeof(DateTime?))
                    updatedAt.SetValue(entity, DateTime.UtcNow);

                await _context.SaveChangesAsync();

                return entity;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }

        public async Task<T?> Delete(T entity)
        {
            try
            {
                _context.Set<T>().Remove(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

    }
}
