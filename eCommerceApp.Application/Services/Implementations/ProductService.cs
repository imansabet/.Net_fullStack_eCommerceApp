﻿using AutoMapper;
using eCommerceApp.Application.DTOs;
using eCommerceApp.Application.DTOs.Product;
using eCommerceApp.Application.Services.Interfaces;
using eCommerceApp.Domain.Entities;
using eCommerceApp.Domain.Interfaces;

namespace eCommerceApp.Application.Services.Implementations;

public class ProductService(IGeneric<Product> productInterface,IMapper mapper) : IProductService 
{
    public async Task<ServiceResponse> AddAsync(CreateProduct product)
    {
        var mappedData = mapper.Map<Product>(product);

        int result = await productInterface.AddAsync(mappedData);
        return result > 0 ? new ServiceResponse(true, "Product Added Successfully")
         : new ServiceResponse(false, "Failed To Add Product !");
    }

    public async Task<ServiceResponse> DeleteAsync(Guid id)
    {
        int result = await productInterface.DeleteAsync(id);
        return result > 0 ? new ServiceResponse(true, "Product Removed Successfully")
            : new ServiceResponse(false, "Failed To Delete Product !");
    }

    public async Task<IEnumerable<GetProduct>> GetAllAsync()
    {
        var rawData = await productInterface.GetAllAsync();
        if (!rawData.Any()) return [];

        return mapper.Map<IEnumerable<GetProduct>>(rawData);
        
    }

    public async Task<GetProduct> GetByIdAsync(Guid id)
    {
        var rawData = await productInterface.GetByIdAsync(id);
        if (rawData == null) return new GetProduct();

        return mapper.Map<GetProduct>(rawData);

    }

    public async Task<ServiceResponse> UpdateAsync(UpdateProduct product)
    {

        var mappedData = mapper.Map<Product>(product);

        int result = await productInterface.UpdateAsync(mappedData);
        return result > 0 ? new ServiceResponse(true, "Product Updated Successfully")
         : new ServiceResponse(false, "Failed To Update Product !");
    }
}
