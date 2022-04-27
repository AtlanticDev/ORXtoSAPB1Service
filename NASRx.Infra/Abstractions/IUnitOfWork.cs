using System;

namespace NASRx.Infra.Abstractions
{
    public interface IUnitOfWork : IDisposable
    {
        void BeginTransaction();

        void CommitTransaction();

        void RollbackTransaction();
    }
}