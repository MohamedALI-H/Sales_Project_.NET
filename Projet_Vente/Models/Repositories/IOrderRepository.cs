using Microsoft.AspNetCore.Identity;
using Projet_Vente.Models;
using Projet_Vente.Models.Repositories;
using Projet_Vente.Models.ViewModels;
using System.Collections.Generic;

namespace Projet_Vente.Services
{
    public interface IOrderRepository
    {
        IEnumerable<Order> GetAllOrders();
        void PlaceOrder(string userId, List<CartItem> cartItems);
        IEnumerable<Order> GetNonDeliveredOrders();
        IEnumerable<Order> GetOrdersByUserId(string userId);
        IEnumerable<LoyalUserViewModel> GetMostLoyalUsers();
    }
}
