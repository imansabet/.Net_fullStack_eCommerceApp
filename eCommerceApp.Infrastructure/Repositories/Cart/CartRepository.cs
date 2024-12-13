using eCommerceApp.Domain.Entities.Cart;
using eCommerceApp.Domain.Interfaces.Cart;
using eCommerceApp.Infrastructure.Data;

namespace eCommerceApp.Infrastructure.Repositories.Cart;

public class CartRepository(AppDbContext context) : ICart
{
    public async Task<int> SaveCheckoutHistory(IEnumerable<Achieve> checkouts)
    {
        context.CheckoutAchieves.AddRange(checkouts);
        return await context.SaveChangesAsync();
    }
}
