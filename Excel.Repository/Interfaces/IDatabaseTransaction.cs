using System;

namespace Excel.Repository.Interfaces
{
    public interface IDatabaseTransaction : IDisposable
    {
        void Commit();
        void Rollback();
    }
}