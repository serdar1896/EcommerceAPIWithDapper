using CoreProject.DataLayer.Infrastructure;
using CoreProject.DataLayer.Repository;
using CoreProject.Entities.Models;
using CoreProject.Entities.VMModels;
using System.Linq;
using System.Threading.Tasks;

namespace CoreProject.BusinessLayer
{
    public class ProductService : BaseRepository<Products>
    {
        public ProductService(IUnitOfWork unitofwork) : base(unitofwork)
        {
        }
        public async Task<ServiceResponse<Products>> GetShowOnPageProducts()
        {
            ServiceResponse<Products> prod;
            using (_unitOfWork)
            {
                var prodlist = await GetAllAsync();
                var t = prodlist.List.FirstOrDefault(x => x.ShowOnHomePage == true);
                prod = await GetByParamAsync(new { Id = t.Id, Status = true });

            }
            return prod;
        }


    }
}
