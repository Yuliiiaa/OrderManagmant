using System;
using System.Collections.Generic;
using Xunit;

public class OrderRepositoryTests
{
    private readonly string connectionString = "Server=localhost;Database=OrderManagmantDB0.1;Trusted_Connection=True;";
    private readonly OrderRepository orderRepository;

    public OrderRepositoryTests()
    {
        orderRepository = new OrderRepository(connectionString);
    }

    [Fact]
    public void CreateOrder_ShouldAddNewOrder()
    {
        var newOrder = new Order
        {
            UserId = 1,
            OrderDate = DateTime.Now,
            Status = "Прийняте",
            TotalAmount = 100.00m
        };

        orderRepository.CreateOrder(newOrder);

        var orders = orderRepository.GetOrders(null, null, "Прийняте");
        Assert.Contains(orders, o => o.Status == "Прийняте");
    }

    [Fact]
    public void UpdateOrderStatus_ShouldChangeOrderStatus()
    {
        var order = orderRepository.GetOrders(null, null, null)[0];
        var oldStatus = order.Status;
        orderRepository.UpdateOrderStatus(order.OrderId, "Оплачене", 1);

        var updatedOrder = orderRepository.GetOrders(null, null, null).First(o => o.OrderId == order.OrderId);
        Assert.NotEqual(oldStatus, updatedOrder.Status);
        Assert.Equal("Оплачене", updatedOrder.Status);
    }

    [Fact]
    public void DeleteOrder_ShouldRemoveOrder()
    {
        var order = orderRepository.GetOrders(null, null, null).First();
        orderRepository.DeleteOrder(order.OrderId);

        var orders = orderRepository.GetOrders(null, null, null);
        Assert.DoesNotContain(orders, o => o.OrderId == order.OrderId);
    }
}
