using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Excel.Repository.Interfaces
{
    public interface IUnitOfWork<TContext> : IDisposable where TContext : DbContext
    {
        IRepositoryBase<TEntity> Repository<TEntity>() where TEntity : class, new();
        Task<int> SaveChanges();
        IDatabaseTransaction BeginTransaction();
    }
}