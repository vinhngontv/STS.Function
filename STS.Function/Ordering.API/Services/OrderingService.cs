using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using Ordering.API.Model;

namespace Ordering.API.Services;

public interface IOrderingService
{
    Task<Order> PlaceOrderAsync(Order order);
    Task<List<Order>> GetOrderAsync();
    Task<Order> GetOrderAsync(Guid id);
    Task Init(ExecutionContext context);
}

public class OrderingService : IOrderingService
{
    private static List<Order> _orders;
    private IExecutionContext _context;

    public List<Order> Orders
    {
        get
        {
            if (_orders != null) return _orders;
            var ordersFile = Path.Combine(_context.FunctionAppDirectory, "orders.json");
            var ordersJson = File.ReadAllText(ordersFile);

            _orders = JsonConvert.DeserializeObject<List<Order>>(ordersJson);

            return _orders;
        }
    }

    public Task<List<Order>> GetOrderAsync()
    {
        return Task.FromResult(Orders);
    }

    public Task<Order> GetOrderAsync(Guid id)
    {
        var order = Orders.FirstOrDefault(o => o.Id == id);

        return Task.FromResult(order);
    }

    public Task Init(ExecutionContext context)
    {
        _context = new FuncExecutionContext
        {
            InvocationId = context.InvocationId,
            FunctionAppDirectory = context.FunctionAppDirectory,
            FunctionDirectory = context.FunctionDirectory,
            FunctionName = context.FunctionName
        };

        return Task.CompletedTask;
    }

    public Task<Order> PlaceOrderAsync(Order order)
    {
        order.Id = Guid.NewGuid();
        order.OrderDate = DateTime.Now;

        _orders.Add(order);

        return Task.FromResult(order);
    }
}