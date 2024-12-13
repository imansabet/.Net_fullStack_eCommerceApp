using AutoMapper;
using eCommerceApp.Application.DTOs;
using eCommerceApp.Application.DTOs.Cart;
using eCommerceApp.Application.Services.Interfaces.Cart;
using eCommerceApp.Domain.Entities;
using eCommerceApp.Domain.Entities.Cart;
using eCommerceApp.Domain.Interfaces;
using eCommerceApp.Domain.Interfaces.Cart;

namespace eCommerceApp.Application.Services.Implementations.Cart;

public class CartService
    (ICart cartInterface ,IMapper mapper , IPaymentService paymentService,
    IPaymentMethodService  paymentMethodService,IGeneric<Product> productInterface) : ICartService
{
    public async Task<ServiceResponse> Checkout(Checkout checkout, string userId)
    {
        var (products, totalAmount) = await GetCartTotalAmount(checkout.Carts);
        var paymentMethods = await paymentMethodService.GetPaymentMethods();
       
        if(checkout.PaymentMethodId == paymentMethods.FirstOrDefault()!.Id) 
             return await paymentService.Pay(totalAmount, products, checkout.Carts);
        else
            return new ServiceResponse(false, "Invalid Payment Method");
    }

    public async Task<ServiceResponse> SaveCheckoutHistory(IEnumerable<CreateAchieve> achieves)
    {
        var mappedData = mapper.Map<IEnumerable<Achieve>>(achieves);
        var result = await cartInterface.SaveCheckoutHistory(mappedData);
        return result > 0 ? new ServiceResponse(true, "Checkout Achieved")
            : new ServiceResponse(false, "Error occurred in Saving");
    }

    private async Task<(IEnumerable<Product> , decimal)> GetCartTotalAmount(IEnumerable<ProcessCart> carts) 
    {
        if (!carts.Any()) return ([], 0);
        var products = await productInterface.GetAllAsync();
        if(!products.Any()) return ([], 0);

        var cartProducts =
            carts
            .Select(cartItem => products.FirstOrDefault(p => p.Id == cartItem.ProductId))
            .Where(product => product != null)
            .ToList();

        var totalAmount =
            carts
            .Where(cartItem => cartProducts.Any(p => p.Id == cartItem.ProductId))
            .Sum(cartItem => cartItem.Quantity * cartProducts.First(p => p.Id == cartItem.ProductId)!.Price);
        
        return (cartProducts!,totalAmount);
    }
}
