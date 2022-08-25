using Mango.Services.ProductAPI.Models.Dto;
using Mango.Services.ProductAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ProductAPI.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductAPIController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private ResponseDto _response;

        public ProductAPIController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
            this._response = new ResponseDto();
        }
        [Authorize]
        [HttpGet]
        public async Task<object> Get()
        {
            try
            {
                _response.Result = await _productRepository.GetProducts();

            }catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessage.Add(ex.Message);
            }
            return _response;
        }
        [Authorize]
        [HttpGet("{id}")]
        public async Task<object> Get(int id)
        {
            try
            {
                _response.Result = await _productRepository.GetProductById(id);

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessage.Add(ex.Message);
            }
            return _response;
        }
        [Authorize]
        [HttpPost]
        public async Task<object> Post(ProductDto product)
        {
            try
            {
                _response.Result = await _productRepository.CreateUpdateProduct(product);

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessage.Add(ex.Message);
            }
            return _response;
        }
        [Authorize]
        [HttpPut]
        public async Task<object> Put(ProductDto product)
        {
            try
            {
                _response.Result = await _productRepository.CreateUpdateProduct(product);

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessage.Add(ex.Message);
            }
            return _response;
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        [Route("{id}")]
        public async Task<object> Delete(int id)
        {
            try
            {
                _response.Result = await _productRepository.DeleteProduct(id);

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessage.Add(ex.Message);
            }
            return _response;
        }
    }
}
