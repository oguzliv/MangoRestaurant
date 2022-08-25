using Mango.Web.Models.Dto;

namespace Mango.Web.Services.IServices
{
    public interface IProductService
    {
        Task<T> GetAllProductsAsync<T>();
        Task<T> GetProductByIdAsync<T>(int id);
        Task<T> CreateProductAsync<T>(ProductDto Dto);
        Task<T> UpdateProductAsync<T>(ProductDto Dto);
        Task<T> DeleteProductAsync<T>(int id);
    }
}
