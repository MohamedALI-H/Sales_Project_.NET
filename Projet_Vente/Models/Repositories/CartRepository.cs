using System.Collections.Generic;
using System.Linq;
using Projet_Vente.Models.Repositories;
namespace Projet_Vente.Models.Repositories;
public class CartRepository : ICartRepository
{
    public void AddToCart(int itemId, string itemName, decimal price, List<CartItem> cart)
    {
        var cartItem = cart.FirstOrDefault(i => i.ItemId == itemId);

        if (cartItem == null)
        {
            cart.Add(new CartItem
            {
                ItemId = itemId,
                ItemName = itemName,
                Price = price,
                Quantity = 1
            });
        }
        else
        {
            cartItem.Quantity++;
        }
    }

    public List<CartItem> GetCartItems(List<CartItem> cart)
    {
        return cart ?? new List<CartItem>();
    }

    public void RemoveFromCart(int itemId, List<CartItem> cart)
    {
        var cartItem = cart.FirstOrDefault(i => i.ItemId == itemId);

        if (cartItem != null)
        {
            cart.Remove(cartItem);
        }
    }

    public void ClearCart(List<CartItem> cart)
    {
        cart.Clear();
    }
}
