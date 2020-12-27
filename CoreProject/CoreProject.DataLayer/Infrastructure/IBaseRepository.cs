using CoreProject.Entities;
using CoreProject.Entities.Infrastructure;
using CoreProject.Entities.Models;
using CoreProject.Entities.VMModels;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace CoreProject.DataLayer.Infrastructure
{
    public interface IBaseRepository<T> where T: BaseEntity, new() 
    {
        Task<ServiceResponse<T>> GetAllAsync();
        Task<ServiceResponse<T>> DeleteRowAsync(int id);
        Task<ServiceResponse<T>> GetByIdAsync(int id);
        Task<ServiceResponse<T>> GetByParamAsync(object param);
        Task<ServiceResponse<T>> InsertRangeAsync(IEnumerable<T> list);
        Task<ServiceResponse<T>> UpdateAsync(T entity);
        Task<ServiceResponse<T>> InsertAsync(T entity);
        Task<List<Products>> GetProductsOneToMany();
        Task<IEnumerable<ProdAndCat>> GetForModel();

    }
}
