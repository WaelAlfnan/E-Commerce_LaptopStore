using Store.BLL.DTOs.CartDTOs;

namespace Store.BLL.Interfaces
{
    public interface ICartService
    {
        Task<CartDetailsDTO> GetUserCart(int userId);
        Task<CartDetailsDTO> AddItemToCart(int userId, CreateCartItemDTO itemDto);
        Task<CartDetailsDTO> UpdateCartItem(int userId, int productId, UpdateCartItemDTO itemDto);
        Task<CartDetailsDTO> RemoveItemFromCart(int userId, int productId);
        Task ClearCart(int userId);
    }
} 
