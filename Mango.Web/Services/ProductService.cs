using Mango.Web.Models.Dto;
using Mango.Web.Services.IServices;

namespace Mango.Web.Services
{
    public class ProductService : BaseService, IProductService
    {
        private readonly IHttpClientFactory _clientFactory;
        public ProductService(IHttpClientFactory httpClient) : base(httpClient)
        {
            _clientFactory = httpClient;
        }

        public async Task<T> CreateProductAsync<T>(ProductDto Dto)
        {
            return await this.SendAsync<T>(new Models.ApiRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = Dto,
                Url = SD.ProdsuctAPIBase + "/api/products",
                AccessToken = ""
            }); ;
        }

        public async Task<T> DeleteProductAsync<T>(int id)
        {
            return await this.SendAsync<T>(new Models.ApiRequest()
            {
                ApiType = SD.ApiType.DELETE,
                Url = SD.ProdsuctAPIBase + "/api/products/" + id,
                AccessToken = ""
            }); ;
        }

        public async Task<T> GetAllProductsAsync<T>()
        {
            return await this.SendAsync<T>(new Models.ApiRequest()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.ProdsuctAPIBase + "/api/products",
                AccessToken = ""
            }); ;
        }

        public async Task<T> GetProductByIdAsync<T>(int id)
        {
            return await this.SendAsync<T>(new Models.ApiRequest()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.ProdsuctAPIBase + "/api/products/" + id,
                AccessToken = ""
            }); ;
        }

        public async Task<T> UpdateProductAsync<T>(ProductDto Dto)
        {
            return await this.SendAsync<T>(new Models.ApiRequest()
            {
                ApiType = SD.ApiType.PUT,
                Data = Dto,
                Url = SD.ProdsuctAPIBase + "/api/products/",
                AccessToken = ""
            }); ;
        }
    }
}
