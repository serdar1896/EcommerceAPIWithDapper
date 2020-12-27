using AutoMapper;
using CoreProject.DataLayer.Infrastructure;
using CoreProject.Entities.Models;
using CoreProject.Entities.VMModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using CoreProject.BusinessLayer;
using CoreProject.DataLayer.CacheService;

namespace CoreProject.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IMapper _mapper;
        private CustomerService _customerService;

        public UserController(IMapper mapper,CustomerService customerService)
        {
            _mapper = mapper;
            _customerService = customerService;
        }

        [HttpGet("GetAll")]
        public async Task<ServiceResponse<Customers>> Get()
        {
            return await _customerService.GetAllAsync();
        }

        [HttpGet("GetById/{id}")]
        public async Task<ServiceResponse<Customers>> Get(int? id)
        {
            return await _customerService.GetByIdAsync(id.Value);
        }
        [HttpGet("GetUser")]
        public async Task<ServiceResponse<Customers>> GetUser(string email, string password)
        {
            return await _customerService.GetUser(email,password); 
        }

        [HttpDelete("DeleteUser/{id}")]
        public async Task<ServiceResponse<Customers>> Delete(int? id)
        {
            return await _customerService.DeleteRowAsync(id.Value);
        }


        [HttpPost("InsertUser")]
        public async Task<ServiceResponse<Customers>> InsertUser(Customers customer)
        {
            return await _customerService.AddUser(customer);
        }

        [HttpPut("UpdateUser")]
        public async Task<ServiceResponse<Customers>> Update(Customers customer)
        {
            return await _customerService.UpdateUser(customer);
        }


    }
}
