using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Projet_Vente.Models;
using Projet_Vente.Models.Repositories;
using Projet_Vente.Services;

namespace Projet_Vente.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartRepository _cartService;
        private readonly IOrderRepository _orderService;
        private readonly IItemRepository _itemService;
        private readonly UserManager<IdentityUser> _userManager;

        public CartController(ICartRepository cartService, IItemRepository itemService, IOrderRepository orderService, UserManager<IdentityUser> userManager)
        {
            _cartService = cartService;
            _orderService = orderService;
            _userManager = userManager;
            _itemService = itemService;
        }

        public IActionResult Index(List<CartItem> cart)
        {
            var cartItems = _cartService.GetCartItems(cart);
            return View(cartItems);
        }

        [HttpPost]
        public IActionResult AddToCart(int itemId, string itemName, decimal price, List<CartItem> cart)
        {
            if (cart == null)
            {
                cart = new List<CartItem>();
            }

            _cartService.AddToCart(itemId, itemName, price, cart);
            return RedirectToAction("Index", new { cart });
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int itemId, List<CartItem> cart)
        {
            if (cart == null)
            {
                cart = new List<CartItem>();
            }

            _cartService.RemoveFromCart(itemId, cart);
            return RedirectToAction("Index", new { cart });
        }

        [HttpPost]
        public IActionResult ConfirmCart(List<CartItem> cart)
        {
            if (cart == null || !cart.Any())
            {
                return RedirectToAction("Index", "Orders");
            }

            var userId = _userManager.GetUserId(User);

            _orderService.PlaceOrder(userId, cart);
            cart.Clear();

            return RedirectToAction("Index", "Orders");
        }
    }
}
