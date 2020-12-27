using CoreProject.DataLayer.CacheService;
using CoreProject.DataLayer.Infrastructure;
using CoreProject.DataLayer.Repository;
using CoreProject.Entities.Models;
using CoreProject.Entities.VMModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoreProject.BusinessLayer
{
    public interface ICustomerService
    {
        Task<ServiceResponse<Customers>> GetUserByRoleId(int id);
        Task<ServiceResponse<Customers>> GetUser(string email, string password);
    }
}
