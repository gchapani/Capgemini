using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Excel.Repository.Interfaces
{
    public interface IRepositoryBase<TEntity> : IDisposable where TEntity : class, new()
    {
        Task<IEnumerable<TEntity>> GetAll(string includeExpressions = null);
        Task<TEntity> GetById(int id);
        void Add(TEntity entidade);
        void AddRange(IEnumerable<TEntity> entidade);
    }
}