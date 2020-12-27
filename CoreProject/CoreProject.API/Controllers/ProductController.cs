using AutoMapper;
using CoreProject.BusinessLayer;
using CoreProject.DataLayer.Infrastructure;
using CoreProject.Entities.Models;
using CoreProject.Entities.VMModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

/*
 * -Get by param generic fonksiyon +
   -Change kontrol'e göre Update
   -isdelete eklencek modellere ve delete böyle olacak.
   -Transication bulk insert vs
   -Store Procedure ile QueryMultiple kullan
   -InnerJoin 
    
     -LOG MANAGER hangi katmanda +
     -View Models hangi katmanda çoklku kullanım
     -Enumlar hnagi katman.
     -
     */
namespace CoreProject.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IMapper _mapper;
        public IUnitOfWork _unitofwork;
        ProductService productService;

        public ProductController(IUnitOfWork unitofwork, IMapper mapper)
        {
            _unitofwork = unitofwork;
            _mapper = mapper;
            productService = new ProductService(_unitofwork);
        }

        [HttpGet("GetAll")]
        public async Task<ServiceResponse<VMProducts>> GetAll()
        {
            var products = await productService.GetAllAsync();
            var vmProducts = _mapper.Map<ServiceResponse<VMProducts>>(products);
            return vmProducts;
        }

        [HttpGet("GetById/{id}")]
        public async Task<string> GetById(int? id)
        {
            var product = await  _unitofwork.GetRepository<Products>().GetByIdAsync(id.Value);
            var vmProduct = _mapper.Map<ServiceResponse<VMProducts>>(product);
            #region ForJonvertJson
            var serializerOptions = new JsonSerializerOptions();
            serializerOptions.Converters.Add(new JsonStringEnumConverter());
            var json = JsonSerializer.Serialize(vmProduct, serializerOptions);
            #endregion
            return json;

        }

        [HttpDelete("DeleteProduct/{id}")]
        public async Task<ServiceResponse<Products>> DeleteProduct(int? id)
        {
            return await productService.DeleteRowAsync(id.Value);
            /*await _unitofwork.GetRepository<Products>().DeleteRowAsync(id.Value);  */
        }

        [HttpPost("InsertProduct")]
        public async Task<ServiceResponse<Products>> InsertProduct(Products product)
        {
            return await productService.InsertAsync(product);
        }

        [HttpPost("InsertRangeProduct")]
        public async Task<ServiceResponse<Products>> InsertRangeProduct(IEnumerable<Products> products)
        {
            return await productService.InsertRangeAsync(products);
        }

        [HttpPut("UpdateProduct")]
        public async Task<ServiceResponse<Products>> UpdateProduct(Products product)
        {
            return await productService.UpdateAsync(product);
        }

    }
}

