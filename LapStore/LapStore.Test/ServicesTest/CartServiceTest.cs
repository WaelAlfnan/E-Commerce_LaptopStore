using LapStore.BLL.Services;
using LapStore.BLL.DTOs.CartDTOs;
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
    public class CartServiceTest
    {
        private readonly Mock<ICartRepository> _cartRepoMock = new();
        private readonly Mock<IProductRepository> _productRepoMock = new();
        private readonly Mock<IUserRepository> _userRepoMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly CartService _service;

        public CartServiceTest()
        {
            _service = new CartService(_cartRepoMock.Object, _productRepoMock.Object, _userRepoMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task GetUserCart_ReturnsCartDetails()
        {
            var userId = 1;
            var cart = new Cart { UserId = userId, cartItems = new List<CartItem>(), user = new User { UserName = "TestUser" } };
            _cartRepoMock.Setup(r => r.GetUserCart(userId)).ReturnsAsync(cart);
            var result = await _service.GetUserCart(userId);
            Assert.NotNull(result);
            Assert.Equal(userId, result.UserId);
            Assert.Equal("TestUser", result.UserName);
        }

        [Fact]
        public async Task AddItemToCart_AddsItemAndReturnsUpdatedCart()
        {
            var userId = 1;
            var productId = 2;
            var cart = new Cart { UserId = userId, cartItems = new List<CartItem>(), user = new User { UserName = "TestUser" } };
            var product = new Product { Id = productId, Name = "Laptop", Price = 1000 };
            _cartRepoMock.Setup(r => r.GetUserCart(userId)).ReturnsAsync(cart);
            _productRepoMock.Setup(r => r.GetById(productId)).Returns(product);
            var itemDto = new CreateCartItemDTO { ProductId = productId, Quantity = 1 };
            var result = await _service.AddItemToCart(userId, itemDto);
            Assert.NotNull(result);
            Assert.Equal(userId, result.UserId);
            _cartRepoMock.Verify(r => r.Update(It.IsAny<Cart>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task AddItemToCart_ThrowsException_WhenProductNotFound()
        {
            var userId = 1;
            var productId = 2;
            var cart = new Cart { UserId = userId, cartItems = new List<CartItem>() };
            _cartRepoMock.Setup(r => r.GetUserCart(userId)).ReturnsAsync(cart);
            _productRepoMock.Setup(r => r.GetById(productId)).Returns((Product)null);
            var itemDto = new CreateCartItemDTO { ProductId = productId, Quantity = 1 };
            await Assert.ThrowsAsync<ArgumentException>(() => _service.AddItemToCart(userId, itemDto));
        }

        [Fact]
        public async Task UpdateCartItem_UpdatesQuantity()
        {
            var userId = 1;
            var productId = 2;
            var cartItem = new CartItem { ProductId = productId, Quantity = 1, product = new Product { Name = "Laptop", Price = 1000 } };
            var cart = new Cart { UserId = userId, cartItems = new List<CartItem> { cartItem }, user = new User { UserName = "TestUser" } };
            _cartRepoMock.Setup(r => r.GetUserCart(userId)).ReturnsAsync(cart);
            var updateDto = new UpdateCartItemDTO { Quantity = 5 };
            var result = await _service.UpdateCartItem(userId, productId, updateDto);
            Assert.Equal(5, result.CartItems.First().Quantity);
            _cartRepoMock.Verify(r => r.Update(It.IsAny<Cart>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateCartItem_ThrowsException_WhenCartNotFound()
        {
            _cartRepoMock.Setup(r => r.GetUserCart(It.IsAny<int>())).ReturnsAsync((Cart)null);
            await Assert.ThrowsAsync<ArgumentException>(() => _service.UpdateCartItem(1, 2, new UpdateCartItemDTO { Quantity = 1 }));
        }

        [Fact]
        public async Task RemoveItemFromCart_RemovesItem()
        {
            var userId = 1;
            var productId = 2;
            var cartItem = new CartItem { ProductId = productId, Quantity = 1, product = new Product { Name = "Laptop", Price = 1000 } };
            var cart = new Cart { UserId = userId, cartItems = new List<CartItem> { cartItem }, user = new User { UserName = "TestUser" } };
            _cartRepoMock.Setup(r => r.GetUserCart(userId)).ReturnsAsync(cart);
            var result = await _service.RemoveItemFromCart(userId, productId);
            Assert.Empty(result.CartItems);
            _cartRepoMock.Verify(r => r.Update(It.IsAny<Cart>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task RemoveItemFromCart_ThrowsException_WhenCartNotFound()
        {
            _cartRepoMock.Setup(r => r.GetUserCart(It.IsAny<int>())).ReturnsAsync((Cart)null);
            await Assert.ThrowsAsync<ArgumentException>(() => _service.RemoveItemFromCart(1, 2));
        }

        [Fact]
        public async Task ClearCart_ClearsAllItems()
        {
            var userId = 1;
            var cart = new Cart { UserId = userId, cartItems = new List<CartItem> { new CartItem { ProductId = 2, Quantity = 1 } } };
            _cartRepoMock.Setup(r => r.GetUserCart(userId)).ReturnsAsync(cart);
            await _service.ClearCart(userId);
            Assert.Empty(cart.cartItems);
            _cartRepoMock.Verify(r => r.Update(It.IsAny<Cart>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Once);
        }
    }
} 