using System;
namespace Projet_Vente.Models.Repositories
{
    public class CartItem
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}

