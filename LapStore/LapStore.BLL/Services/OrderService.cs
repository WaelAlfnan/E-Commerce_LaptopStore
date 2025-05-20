using LapStore.BLL.DTOs.OrderDTOs;
using LapStore.BLL.Interfaces;
using LapStore.DAL;
using LapStore.DAL.Data.Entities;
using LapStore.DAL.Repositories;

namespace LapStore.BLL.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<OrderDetailsDTO>> GetUserOrders(int userId)
        {
            var orders = await _orderRepository.GetUserOrders(userId);
            return orders.Select(o => MapToOrderDetailsDTO(o));
        }

        public async Task<OrderDetailsDTO> GetOrderDetails(int orderId)
        {
            var order = await _orderRepository.GetOrderWithItems(orderId);
            if (order == null)
                throw new ArgumentException("Order not found");

            return MapToOrderDetailsDTO(order);
        }

        public async Task<IEnumerable<OrderDetailsDTO>> GetOrdersByStatus(OrderStatus status)
        {
            var orders = await _orderRepository.GetOrdersByStatus(status);
            return orders.Select(o => MapToOrderDetailsDTO(o));
        }

        public async Task<OrderDetailsDTO> CreateOrder(int userId, CreateOrderDTO orderDto)
        {
            var user =  _userRepository.GetById(userId);
            if (user == null)
                throw new ArgumentException("User not found");

            var order = new Order
            {
                UserId = userId,
                Date = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                orderItems = new List<OrderItem>()
            };

            decimal totalAmount = 0;
            foreach (var item in orderDto.OrderItems)
            {
                var product =  _productRepository.GetById(item.ProductId);
                if (product == null)
                    throw new ArgumentException($"Product with ID {item.ProductId} not found");

                var orderItem = new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price
                };

                order.orderItems.Add(orderItem);
                totalAmount += product.Price * item.Quantity;
            }

            order.TotalAmount = totalAmount;

            await _orderRepository.AddAsync(order);
            await _unitOfWork.CompleteAsync();

            // Fetch the complete order with related entities
            var completeOrder = await _orderRepository.GetOrderWithItems(order.Id);
            return MapToOrderDetailsDTO(completeOrder);
        }

        public async Task<OrderDetailsDTO> UpdateOrderStatus(int orderId, UpdateOrderStatusDTO statusDto)
        {
            var order = await _orderRepository.GetOrderWithItems(orderId);
            if (order == null)
                throw new ArgumentException("Order not found");

            order.Status = statusDto.Status;
             _orderRepository.Update(order);
            await _unitOfWork.CompleteAsync();

            return MapToOrderDetailsDTO(order);
        }

        private OrderDetailsDTO MapToOrderDetailsDTO(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            return new OrderDetailsDTO
            {
                Id = order.Id,
                Date = order.Date,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                UserName = order.user?.UserName ?? "Unknown User",
                UserEmail = order.user?.Email ?? "No Email",
                OrderItems = order.orderItems?.Select(oi => new OrderItemDetailsDTO
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.product?.Name ?? "Unknown Product",
                    ProductImage = oi.product?.productImages?.FirstOrDefault()?.URL ?? "default-image-url",
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    TotalPrice = oi.UnitPrice * oi.Quantity
                }).ToList() ?? new List<OrderItemDetailsDTO>()
            };
        }
    }
} 