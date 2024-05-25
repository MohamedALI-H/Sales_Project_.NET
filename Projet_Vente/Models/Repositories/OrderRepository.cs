using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Projet_Vente.Models;
using Projet_Vente.Models.Repositories;
using Projet_Vente.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace Projet_Vente.Services
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Order> GetAllOrders()
        {
            return _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Item)
                .ToList();
        }


        public void PlaceOrder(string userId, List<CartItem> cartItems)
        {
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                IsDelivered = false,
                OrderItems = cartItems.Select(ci => new OrderItem
                {
                    ItemId = ci.ItemId,
                    Quantity = ci.Quantity,
                    Price = ci.Price
                }).ToList()
            };

            _context.Orders.Add(order);
            _context.SaveChanges();
        }


        public IEnumerable<Order> GetNonDeliveredOrders()
        {
            // Assuming there is a field 'IsDelivered' in the Order model
            return _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Item)
                .Where(o => !o.IsDelivered)
                .ToList();
        }

        public IEnumerable<Order> GetOrdersByUserId(string userId)
        {
            return _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Item)
                .Where(o => o.UserId == userId)
                .ToList();
        }

        public IEnumerable<LoyalUserViewModel> GetMostLoyalUsers()
        {
            var loyalUsers = _context.Orders
                .GroupBy(o => o.User)
                .Select(g => new LoyalUserViewModel
                {
                    UserId = g.Key.Id,
                    UserName = g.Key.UserName,
                    OrderCount = g.Count()
                })
                .OrderByDescending(u => u.OrderCount)
                .ToList();

            return loyalUsers;
        }
    }
}
