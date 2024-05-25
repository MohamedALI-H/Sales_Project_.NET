using System;
namespace Projet_Vente.Models.ViewModels
{
    public class EditItemModel : CreateItemModel
    {
        public int Id { get; set; }
        public string ExistingImagePath { get; set; }
    }
}

