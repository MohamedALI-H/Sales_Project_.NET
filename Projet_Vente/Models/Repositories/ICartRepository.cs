using System;
using System.Security.Cryptography;

namespace Projet_Vente.Models.Repositories
{
	public interface ICartRepository
	{
        void AddToCart(int itemId, string itemName, decimal price, List<CartItem> cart);
      List<CartItem> GetCartItems(List<CartItem> cart);
        void RemoveFromCart(int itemId, List<CartItem> cart);
        void ClearCart(List<CartItem> cart);
    }
}

