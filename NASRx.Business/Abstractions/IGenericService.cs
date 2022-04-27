using NASRx.Model;
using System.Threading.Tasks;

namespace NASRx.Business.Abstractions
{
    public interface IGenericService<TMDL, TPK> where TMDL : Entity<TPK>
    {
        Task<bool> Delete(TMDL entity);

        Task<TMDL> Insert(TMDL entity);

        Task<bool> Update(TMDL entity);
    }
}