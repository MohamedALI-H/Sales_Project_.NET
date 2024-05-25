using System;
using System.Collections.Generic;

namespace Projet_Vente.Models.ViewModels
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }
        public float OrderPrice { get; set; }
        public string UserName { get; set; }
        public DateTime OrderDate { get; set; }
        public List<string> Items { get; set; }
    }
}
