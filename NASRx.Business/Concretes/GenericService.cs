using NASRx.Business.Abstractions;
using NASRx.Infra.Abstractions;
using NASRx.Model;
using NASRx.Repositories.Abstractions;
using System;
using System.Threading.Tasks;

namespace NASRx.Business.Concretes
{
    public class GenericService<TMDL, TPK> : IGenericService<TMDL, TPK> where TMDL : Entity<TPK>
    {
        public GenericService(ILogging logging, IGenericRepository<TMDL, TPK> repository, IUnitOfWork unitOfWork)
        {
            Logging = logging;
            Repository = repository;
            UnitOfWork = unitOfWork;
        }

        protected ILogging Logging { get; private set; }

        protected IGenericRepository<TMDL, TPK> Repository { get; private set; }

        protected IUnitOfWork UnitOfWork { get; private set; }

        public virtual Task<bool> Delete(TMDL entity)
        {
            try
            {
                UnitOfWork.BeginTransaction();
                var result = Repository.Delete(entity);
                UnitOfWork.CommitTransaction();
                return result;
            }
            catch (Exception ex)
            {
                UnitOfWork.RollbackTransaction();
                Logging.LogError(ex);
                throw;
            }
        }

        public virtual async Task<TMDL> Insert(TMDL entity)
        {
            try
            {
                UnitOfWork.BeginTransaction();
                entity = await Repository.Insert(entity);
                UnitOfWork.CommitTransaction();
                return entity;
            }
            catch (Exception ex)
            {
                UnitOfWork.RollbackTransaction();
                Logging.LogError(ex);
                throw;
            }
        }

        public virtual async Task<bool> Update(TMDL entity)
        {
            try
            {
                bool result;
                UnitOfWork.BeginTransaction();
                result = await Repository.Update(entity);
                UnitOfWork.CommitTransaction();
                return result;
            }
            catch (Exception ex)
            {
                UnitOfWork.RollbackTransaction();
                Logging.LogError(ex);
                throw;
            }
        }
    }
}