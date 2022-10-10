using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Basket.API.Model;

namespace Basket.API.Services;

public interface IBasketRepository
{
    Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket);
    Task<CustomerBasket> GetBasketAsync(Guid id);
    Task DeleteBasketAsync(Guid id);
}

public class BasketRepository : IBasketRepository
{
    private static List<CustomerBasket> _baskets;

    public BasketRepository()
    {
        _baskets ??= new List<CustomerBasket>();
    }

    public Task DeleteBasketAsync(Guid id)
    {
        var basket = GetBasketAsync(id).Result;

        if (basket != null)
            _baskets.Remove(basket);

        return Task.CompletedTask;
    }

    public Task<CustomerBasket> GetBasketAsync(Guid id)
    {
        var basket = _baskets.FirstOrDefault(b => b.Id == id);

        return Task.FromResult(basket);
    }

    public Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket)
    {
        var customerBasket = _baskets.FirstOrDefault(b => b.Id == basket.Id);

        if (customerBasket != null)
        {
            customerBasket = basket;
            customerBasket.DateUpdated = DateTime.Now;
        }
        else
        {
            customerBasket = new CustomerBasket
            {
                Id = basket.Id == Guid.Empty ? Guid.NewGuid() : basket.Id,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                Items = basket.Items
            };
            _baskets.Add(customerBasket);
        }

        return Task.FromResult(customerBasket);
    }
}