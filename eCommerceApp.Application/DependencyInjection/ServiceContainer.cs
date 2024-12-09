using eCommerceApp.Application.Mapping;
using eCommerceApp.Application.Services.Implementations;
using eCommerceApp.Application.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace eCommerceApp.Application.DependencyInjection;

public static class ServiceContainer
{
    public static IServiceCollection AddApplicationService(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MappingConfig));

        services.AddScoped<IProductService,ProductService>();
        services.AddScoped<ICategoryService,CategoryService>();

        return services;
    }
}
