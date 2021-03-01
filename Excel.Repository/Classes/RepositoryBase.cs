using EFCore.BulkExtensions;
using Excel.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Excel.Repository.Classes
{
    public class RepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : class, new()
    {
        protected readonly DbContext _context;
        protected readonly DbSet<TEntity> _dbSet;
        public RepositoryBase(DbContext pContext)
        {
            _context = pContext;
            _dbSet = _context.Set<TEntity>();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAll(string includeExpressions = null)
        {
            IQueryable<TEntity> query = _dbSet.AsNoTracking<TEntity>();

            if (includeExpressions != null)
            {
                query = Include(query, includeExpressions);

            }

            return await query.AsNoTracking().ToListAsync<TEntity>();
        }
        public virtual async Task<TEntity> GetById(int id)
        {
            return await _dbSet.FindAsync(id);
        }
        public virtual void Add(TEntity entidade)
        {
            _dbSet.Add(entidade);
        }
        public virtual void AddRange(IEnumerable<TEntity> entidade)
        {
            if (_context.Database.IsSqlServer())
            {
                _dbSet.AddRange(entidade.ToList<TEntity>());
            }
            else
            {
                _dbSet.AddRange(entidade);
            }
        }

        public void Dispose()
        {
            _context?.Dispose();
        }

        private IQueryable<TEntity> Include(IQueryable<TEntity> query, string includeExpressions = null)
        {
            string[] includes = includeExpressions.Replace(" ", "").Split(',');

            foreach (string item in includes)
            {
                query = query.Include(item);
            }

            return query;
        }
    }
}