using AutoMapper;
using eCommerceApp.Application.DTOs;
using eCommerceApp.Application.DTOs.Category;
using eCommerceApp.Application.DTOs.Product;
using eCommerceApp.Application.Services.Interfaces;
using eCommerceApp.Domain.Entities;
using eCommerceApp.Domain.Interfaces;

namespace eCommerceApp.Application.Services.Implementations;

public class CategoryService(IGeneric<Category> categoryInterface , IMapper mapper) : ICategoryService
{
    public async Task<ServiceResponse> AddAsync(CreateCategory category )
    {
        var mappedData = mapper.Map<Category>(category);

        int result = await categoryInterface.AddAsync(mappedData);
        return result > 0 ? new ServiceResponse(true, "Category Added Successfully")
         : new ServiceResponse(false, "Failed To Add Category !");
    }

    public async Task<ServiceResponse> DeleteAsync(Guid id)
    {
        int result = await categoryInterface.DeleteAsync(id);
      

        return result > 0 ? new ServiceResponse(true, "Category Removed Successfully")
            : new ServiceResponse(false, "Failed To Delete Category !");
    }

    public async Task<IEnumerable<GetCategory>> GetAllAsync()
    {
        var rawData = await categoryInterface.GetAllAsync();
        if (!rawData.Any()) return [];

        return mapper.Map<IEnumerable<GetCategory>>(rawData);

    }

    public async Task<GetCategory> GetByIdAsync(Guid id)
    {
        var rawData = await categoryInterface.GetByIdAsync(id);
        if (rawData == null) return new GetCategory();

        return mapper.Map<GetCategory>(rawData);
    }

    public async Task<ServiceResponse> UpdateAsync(UpdateCategory category)
    {
        var mappedData = mapper.Map<Category>(category);

        int result = await categoryInterface.UpdateAsync(mappedData);
        return result > 0 ? new ServiceResponse(true, "Category Updated Successfully")
         : new ServiceResponse(false, "Failed To Update Category !");
    }

   
}
