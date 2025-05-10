using LapStore.BLL.Services;
using LapStore.BLL.DTOs.OrderDTOs;
using LapStore.DAL.Data.Entities;
using LapStore.DAL.Repositories;
using Moq;
using Xunit;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using LapStore.DAL;

namespace LapStore.Test.ServicesTest
{
    public class OrderServiceTest
    {
        private readonly Mock<IOrderRepository> _orderRepoMock = new();
        private readonly Mock<IProductRepository> _productRepoMock = new();
        private readonly Mock<IUserRepository> _userRepoMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly OrderService _service;

        public OrderServiceTest()
        {
            _service = new OrderService(_orderRepoMock.Object, _productRepoMock.Object, _userRepoMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task GetUserOrders_ReturnsOrderDetails()
        {
            var userId = 1;
            var orders = new List<Order> { new Order { Id = 1, UserId = userId, user = new User { UserName = "TestUser", Email = "test@example.com" }, orderItems = new List<OrderItem>() } };
            _orderRepoMock.Setup(r => r.GetUserOrders(userId)).ReturnsAsync(orders);
            var result = await _service.GetUserOrders(userId);
            Assert.Single(result);
            Assert.Equal(userId, result.First().Id);
        }

        [Fact]
        public async Task GetOrderDetails_ReturnsOrderDetails()
        {
            var orderId = 1;
            var order = new Order { Id = orderId, user = new User { UserName = "TestUser", Email = "test@example.com" }, orderItems = new List<OrderItem>() };
            _orderRepoMock.Setup(r => r.GetOrderWithItems(orderId)).ReturnsAsync(order);
            var result = await _service.GetOrderDetails(orderId);
            Assert.NotNull(result);
            Assert.Equal(orderId, result.Id);
        }

        [Fact]
        public async Task GetOrderDetails_ThrowsException_WhenOrderNotFound()
        {
            _orderRepoMock.Setup(r => r.GetOrderWithItems(It.IsAny<int>())).ReturnsAsync((Order)null);
            await Assert.ThrowsAsync<ArgumentException>(() => _service.GetOrderDetails(1));
        }

        [Fact]
        public async Task GetOrdersByStatus_ReturnsOrders()
        {
            var status = OrderStatus.Pending;
            var orders = new List<Order> { new Order { Id = 1, Status = status, user = new User { UserName = "TestUser", Email = "test@example.com" }, orderItems = new List<OrderItem>() } };
            _orderRepoMock.Setup(r => r.GetOrdersByStatus(status)).ReturnsAsync(orders);
            var result = await _service.GetOrdersByStatus(status);
            Assert.Single(result);
            Assert.Equal(status, result.First().Status);
        }

        [Fact]
        public async Task CreateOrder_CreatesOrderAndReturnsDetails()
        {
            var userId = 1;
            var user = new User { Id = userId, UserName = "TestUser", Email = "test@example.com" };
            var product = new Product { Id = 2, Name = "Laptop", Price = 1000 };
            var orderDto = new CreateOrderDTO { OrderItems = new List<CreateOrderItemDTO> { new CreateOrderItemDTO { ProductId = 2, Quantity = 1 } } };
            _userRepoMock.Setup(r => r.GetById(userId)).Returns(user);
            _productRepoMock.Setup(r => r.GetById(2)).Returns(product);
            _orderRepoMock.Setup(r => r.AddAsync(It.IsAny<Order>())).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.CompleteAsync()).Returns((Task<int>)Task.CompletedTask);
            var result = await _service.CreateOrder(userId, orderDto);
            Assert.NotNull(result);
            Assert.Equal(userId, result.UserName == user.UserName ? userId : 0); // UserName check
            _orderRepoMock.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateOrder_ThrowsException_WhenUserNotFound()
        {
            _userRepoMock.Setup(r => r.GetById(It.IsAny<int>())).Returns((User)null);
            var orderDto = new CreateOrderDTO { OrderItems = new List<CreateOrderItemDTO>() };
            await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateOrder(1, orderDto));
        }

        [Fact]
        public async Task CreateOrder_ThrowsException_WhenProductNotFound()
        {
            var userId = 1;
            var user = new User { Id = userId };
            _userRepoMock.Setup(r => r.GetById(userId)).Returns(user);
            _productRepoMock.Setup(r => r.GetById(It.IsAny<int>())).Returns((Product)null);
            var orderDto = new CreateOrderDTO { OrderItems = new List<CreateOrderItemDTO> { new CreateOrderItemDTO { ProductId = 2, Quantity = 1 } } };
            await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateOrder(userId, orderDto));
        }

        [Fact]
        public async Task UpdateOrderStatus_UpdatesStatusAndReturnsOrder()
        {
            var orderId = 1;
            var order = new Order { Id = orderId, Status = OrderStatus.Pending, user = new User { UserName = "TestUser", Email = "test@example.com" }, orderItems = new List<OrderItem>() };
            _orderRepoMock.Setup(r => r.GetOrderWithItems(orderId)).ReturnsAsync(order);
            var statusDto = new UpdateOrderStatusDTO { Status = OrderStatus.Delivered };
            var result = await _service.UpdateOrderStatus(orderId, statusDto);
            Assert.Equal(OrderStatus.Delivered, result.Status);
            _orderRepoMock.Verify(r => r.Update(It.IsAny<Order>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateOrderStatus_ThrowsException_WhenOrderNotFound()
        {
            _orderRepoMock.Setup(r => r.GetOrderWithItems(It.IsAny<int>())).ReturnsAsync((Order)null);
            var statusDto = new UpdateOrderStatusDTO { Status = OrderStatus.Delivered };
            await Assert.ThrowsAsync<ArgumentException>(() => _service.UpdateOrderStatus(1, statusDto));
        }
    }
} 