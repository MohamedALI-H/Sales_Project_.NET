using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projet_Vente.Models;
using Projet_Vente.Models.Repositories;
using Projet_Vente.Models.ViewModels;
using Projet_Vente.Services;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Projet_Vente.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OrdersController : Controller
    {
        private readonly IOrderRepository _orderService;
        private readonly UserManager<IdentityUser> userManager;

        public OrdersController(IOrderRepository orderService, UserManager<IdentityUser> userManager)
        {
            _orderService = orderService;
            this.userManager = userManager;
        }

        public IActionResult Index()
        {
            var orders = _orderService.GetAllOrders();
            return View("~/Views/Admin/Orders/Index.cshtml", orders);
        }

        public IActionResult NonDeliveredOrders()
        {
            var orders = _orderService.GetNonDeliveredOrders();
            return View("~/Views/Admin/Orders/NonDeliveredOrders.cshtml", orders);
        }

        [AllowAnonymous]
        public IActionResult OrdersByUser(string userId)
        {
            var orders = _orderService.GetOrdersByUserId(userId);
            return View(orders);
        }

        public IActionResult MostLoyalUsers()
        {
            var loyalUsers = _orderService.GetMostLoyalUsers();
            return View("~/Views/Admin/Orders/MostLoyalUsers.cshtml", loyalUsers);
        }
    }
}

