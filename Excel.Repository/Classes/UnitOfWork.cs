using Excel.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Excel.Repository.Classes
{
    public class UnitOfWork<TContext> : IUnitOfWork<TContext> where TContext : DbContext
    {
        private readonly TContext _contexto = null;
        private readonly Dictionary<Type, object> _repositories = new Dictionary<Type, object>();

        public UnitOfWork(TContext contexto)
        {
            _contexto = contexto;
        }

        public IRepositoryBase<TEntity> Repository<TEntity>() where TEntity : class, new()
        {
            if (_repositories.Keys.Contains(typeof(TEntity)))
            {
                return _repositories[typeof(TEntity)] as IRepositoryBase<TEntity>;
            }

            IRepositoryBase<TEntity> repo = new RepositoryBase<TEntity>(_contexto);
            _repositories.Add(typeof(TEntity), repo);
            return repo;
        }

        public async Task<int> SaveChanges()
        {
            if (_contexto != null)
            {
                try
                {
                    return await _contexto.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return 0;
        }

        public void Dispose()
        {
            _contexto?.Dispose();
        }

        public IDatabaseTransaction BeginTransaction()
        {
            return new EntityDatabaseTransaction(_contexto);
        }
    }
}