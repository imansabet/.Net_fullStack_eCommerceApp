using eCommerceApp.Application.DTOs.Cart;

namespace eCommerceApp.Application.Services.Interfaces.Cart;

public interface IPaymentMethodService
{
    Task<IEnumerable<GetPaymentMethod>> GetPaymentMethods();
}
