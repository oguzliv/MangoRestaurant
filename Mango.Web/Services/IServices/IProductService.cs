﻿using Mango.Web.Models.Dto;

namespace Mango.Web.Services.IServices
{
    public interface IProductService
    {
        Task<T> GetAllProductsAsync<T>(string token);
        Task<T> GetProductByIdAsync<T>(int id, string token);
        Task<T> CreateProductAsync<T>(ProductDto Dto, string token);
        Task<T> UpdateProductAsync<T>(ProductDto Dto, string token);
        Task<T> DeleteProductAsync<T>(int id, string token);
    }
}
