using NASRx.Infra.Abstractions;
using System;

namespace NASRx.Infra.Concretes
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IContext _context;

        public UnitOfWork(IContext context)
            => _context = context;

        public void BeginTransaction()
            => _context.BeginTransaction();

        public void CommitTransaction()
            => _context.CommitTransaction();

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

        public void RollbackTransaction()
            => _context.RollbackTransaction();
    }
}