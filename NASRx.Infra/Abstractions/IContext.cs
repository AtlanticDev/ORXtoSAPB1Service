
using System;
using System.Data.Common;

namespace NASRx.Infra.Abstractions
{
    public interface IContext : IDisposable
    {
        DbConnection Connection { get; }

        DbTransaction Transaction { get; }

        // ICache Cache { get; }

        void BeginTransaction();

        void CommitTransaction();

        void RollbackTransaction();
    }
}