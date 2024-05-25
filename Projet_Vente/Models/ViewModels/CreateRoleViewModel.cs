using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
namespace Projet_Vente.Models.ViewModels
{
	public class CreateRoleViewModel
	{
        [Required]
        [Display(Name = "Role")]

        public string RoleName { get; set; }
    }
}

