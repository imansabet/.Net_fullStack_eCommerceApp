using eCommerceApp.Application.DTOs;
using eCommerceApp.Application.DTOs.Cart;
using eCommerceApp.Domain.Entities.Cart;

namespace eCommerceApp.Application.Services.Interfaces.Cart;

public interface ICartService
{
    Task<ServiceResponse> SaveCheckoutHistory(IEnumerable<CreateAchieve> achieves);
    Task<ServiceResponse> Checkout(Checkout checkout);

}
