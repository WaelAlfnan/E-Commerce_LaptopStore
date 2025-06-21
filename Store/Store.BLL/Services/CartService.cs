using Store.BLL.DTOs.CartDTOs;
using Store.BLL.Interfaces;
using Store.DAL;
using Store.DAL.Data.Entities;
using Store.DAL.Repositories;

namespace Store.BLL.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CartService(
            ICartRepository cartRepository,
            IProductRepository productRepository,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<CartDetailsDTO> GetUserCart(int userId)
        {
            var cart = await _cartRepository.GetUserCart(userId);
            if (cart == null)
            {
                cart = new Cart { UserId = userId, cartItems = new List<CartItem>() };
                await _cartRepository.AddAsync(cart);
                await _unitOfWork.CompleteAsync(); // Save to get a permanent Id
            }

            return MapToCartDetailsDTO(cart);
        }

        public async Task<CartDetailsDTO> AddItemToCart(int userId, CreateCartItemDTO itemDto)
        {
            var cart = await _cartRepository.GetUserCart(userId);
            bool isNewCart = false;
            if (cart == null)
            {
                cart = new Cart { UserId = userId, cartItems = new List<CartItem>() };
                await _cartRepository.AddAsync(cart);
                await _unitOfWork.CompleteAsync(); // Save to get a permanent Id
                isNewCart = true;
            }

            var product = await _productRepository.GetByIdAsync(itemDto.ProductId);
            if (product == null)
                throw new ArgumentException("Product not found");

            var existingItem = cart.cartItems?.FirstOrDefault(ci => ci.ProductId == itemDto.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += itemDto.Quantity;
            }
            else
            {
                cart.cartItems?.Add(new CartItem
                {
                    ProductId = itemDto.ProductId,
                    Quantity = itemDto.Quantity
                });
            }

            if (!isNewCart)
            {
                _cartRepository.Update(cart);
            }
            await _unitOfWork.CompleteAsync();

            return MapToCartDetailsDTO(cart);
        }

        public async Task<CartDetailsDTO> UpdateCartItem(int userId, int productId, UpdateCartItemDTO itemDto)
        {
            var cart = await _cartRepository.GetUserCart(userId);
            if (cart == null)
                throw new ArgumentException("Cart not found");

            var cartItem = cart.cartItems?.FirstOrDefault(ci => ci.ProductId == productId);
            if (cartItem == null)
                throw new ArgumentException("Item not found in cart");

            cartItem.Quantity = itemDto.Quantity;

            _cartRepository.Update(cart);
            await _unitOfWork.CompleteAsync();

            return MapToCartDetailsDTO(cart);
        }

        public async Task<CartDetailsDTO> RemoveItemFromCart(int userId, int productId)
        {
            var cart = await _cartRepository.GetUserCart(userId);
            if (cart == null)
                throw new ArgumentException("Cart not found");

            var cartItem = cart.cartItems?.FirstOrDefault(ci => ci.ProductId == productId);
            if (cartItem == null)
                throw new ArgumentException("Item not found in cart");

            cart.cartItems?.Remove(cartItem);

            _cartRepository.Update(cart);
            await _unitOfWork.CompleteAsync();

            return MapToCartDetailsDTO(cart);
        }

        public async Task ClearCart(int userId)
        {
            var cart = await _cartRepository.GetUserCart(userId);
            if (cart == null)
                throw new ArgumentException("Cart not found");

            cart.cartItems?.Clear();

            _cartRepository.Update(cart);
            await _unitOfWork.CompleteAsync();
        }

        private CartDetailsDTO MapToCartDetailsDTO(Cart cart)
        {
            var items = cart.cartItems?.Select(ci => new CartItemDetailsDTO
            {
                ProductId = ci.ProductId,
                ProductName = ci.product?.Name ?? string.Empty,
                ProductImage = ci.product?.productImages?.FirstOrDefault()?.URL ?? string.Empty,
                UnitPrice = ci.product?.Price ?? 0,
                Quantity = ci.Quantity,
                TotalPrice = (ci.product?.Price ?? 0) * ci.Quantity
            }).ToList() ?? new List<CartItemDetailsDTO>();

            return new CartDetailsDTO
            {
                Id = cart.Id,
                UserId = cart.UserId,
                UserName = cart.user?.UserName ?? string.Empty,
                CartItems = items,
                TotalAmount = items.Sum(i => i.TotalPrice)
            };
        }
    }
} 
