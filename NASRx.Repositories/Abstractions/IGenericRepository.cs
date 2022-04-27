using NASRx.Model;
using System.Threading.Tasks;

namespace NASRx.Repositories.Abstractions
{
    public interface IGenericRepository<TMDL, TPK> where TMDL : Entity<TPK>
    {
        Task<bool> Delete(TMDL entity);

        Task<TMDL> Insert(TMDL entity);

        Task<bool> Update(TMDL entity);
    }
}