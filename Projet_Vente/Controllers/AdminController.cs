using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Hosting.Internal;
using Projet_Vente.Models;
using Projet_Vente.Models.Repositories;

using Projet_Vente.Models.ViewModels;
using Projet_Vente.Services;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Ventes_En_ligne_Projet.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IUserRepository _userRepository;
      
        private readonly IItemRepository _itemRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICommonService _commonService;
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly RoleManager<IdentityRole> roleManager;

        private readonly UserManager<IdentityUser> userManager;

        public AdminController(IUserRepository userRepository, IItemRepository itemRepository,
            IWebHostEnvironment hostingEnvironment,

         ICommonService commonService, ICategoryRepository categoryRepository, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _userRepository = userRepository;

            _itemRepository = itemRepository;
            this._categoryRepository = categoryRepository;
            this._commonService = commonService;
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.hostingEnvironment = hostingEnvironment;
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            var userRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
            var isAdmin = userRoles.Contains("Admin");

            if (!isAdmin)
            {
                // Log or display error message
                ViewBag.ErrorMessage = "Access Denied. You do not have permission to access this page.";
                return View("Error");
            }

            return View();
        }


        public async Task<IActionResult> Index()
        {

            return View();
        }

        public async Task<IActionResult> Users()
        {
            var users = userManager.Users.ToList();
            var userViewModels = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user);
                userViewModels.Add(new UserViewModel
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = roles.ToList()
                });
            }

            return View("users/users",userViewModels);
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View("roles/CreateRole");
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole role = new IdentityRole { Name = model.RoleName };
                IdentityResult result = await roleManager.CreateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("roles/ListRoles");
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View("roles/ListRoles", model);
        }

        [HttpGet]
        public IActionResult ListRoles()
        {
            var roles = roleManager.Roles;
            return View("roles/ListRoles",roles);
        }

      



        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            // Find the role by Role ID
            var role = await roleManager.FindByIdAsync(id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
                return View("NotFound");
            }
            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name
            };
            // Retrieve all the Users
            foreach (var user in userManager.Users.ToList())
            {
                // If the user is in this role, add the username to
                // Users property of EditRoleViewModel. This model
                // object is then passed to the view for display
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }
            return View("roles/EditRole",model) ;
        }

        public IActionResult Dashboard()
        {
            var categories = _commonService.GetCategories();
            var items = _itemRepository.GetItems();

            var model = categories.Select(c => new CategoryItemCountViewModel
            {
                CategoryId = c.Id,
                CategoryName = c.Name,
                ItemCount = items.Count(i => i.CategoryId == c.Id)
            }).ToList();

            return View("dashboard", model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var role = await roleManager.FindByIdAsync(model.Id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                role.Name = model.RoleName;
                // Update the Role using UpdateAsync
                var result = await roleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("roles/ListRoles");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(""
                    , error.Description);
                }
                return View("roles/ListRoles", model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            ViewBag.roleId = roleId;
            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }
            var model = new List<UserRoleViewModel>();
            foreach (var user in userManager.Users.ToList())
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }
                model.Add(userRoleViewModel);
            }
            return View("roles/EditUsersInRole", model) ;
        }
        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model, string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }
            for (int i = 0; i < model.Count; i++)
            {
                var user = await userManager.FindByIdAsync(model[i].UserId);
                IdentityResult result = null;
                if (model[i].IsSelected && !(await userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!model[i].IsSelected && await userManager.IsInRoleAsync(user, role.Name))
                {
                    result = await userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }
                if (result.Succeeded)
                {
                    if (i < (model.Count - 1))
                        continue;
                    else
                        return RedirectToAction("roles/listRoles"
                        );
                }
            }
            return RedirectToAction("roles/listRoles");
        }

        

       


        // Item management actions

        public IActionResult Items()
        {
            var items = _commonService.GetItems();
            return View("Items/items",items);
        }

        public IActionResult GetItem(int id)
        {
            var item = _commonService.GetItem(id);
            if (item == null)
            {
                return NotFound();
            }
            return View("items/getItem",item);
        }

        public IActionResult CreateItem()
        {
            ViewBag.CategoryId = new SelectList(_commonService.GetCategories(), "Id", "Name");
            return View("Items/CreateItem");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateItem(CreateItemModel model)
        {
            
                string uniqueFileName = null;
                // If the Photo property on the incoming model object is not null, then the user has selected an image to upload.
                if (model.ImagePath != null)
                {
                    // The image must be uploaded to the images folder in wwwroot
                    // To get the path of the wwwroot folder we are using the inject
                    // HostingEnvironment service provided by ASP.NET Core
                    string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "images");
                    // To make sure the file name is unique we are appending a new
                    // GUID value and an underscore to the file name
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImagePath.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    // Use CopyTo() method provided by IFormFile interface to
                    // copy the file to wwwroot/images folder
                    model.ImagePath.CopyTo(new FileStream(filePath, FileMode.Create));
                }
                Item NewItem = new Item
                {
                    Description = model.Description,
                    Name = model.Name,
                    Price = model.Price,
                    CategoryId = model.CategoryId,
                    ImageUrl = uniqueFileName


                };
                 _itemRepository.AddItem(NewItem);
            return RedirectToAction(nameof(Index));
        }






        public IActionResult EditItem(int id)
        {
            var item = _commonService.GetItem(id);
            ViewBag.CategoryId = new SelectList(_commonService.GetCategories(), "Id", "Name");
            if (item == null)
            {
                return NotFound();
            }
            EditItemModel model = new EditItemModel
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,

                ExistingImagePath = item.ImageUrl
            };
            return View("Items/EditItem", model);
                }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditItemModel model)
        {

            if (ModelState.IsValid)
            {
                Item item = _commonService.GetItem(model.Id);
                item.Description = model.Description;
                item.Price = model.Price;
                
                item.CategoryId = model.CategoryId;
                if (model.ImagePath != null)
                {
                    if (model.ExistingImagePath != null)
                    {
                        string filePath = Path.Combine(hostingEnvironment.WebRootPath, "images", model.ExistingImagePath);
                        System.IO.File.Delete(filePath);
                    }
                    item.ImageUrl = ProcessUploadedFile(model);
                }
                Item updatedArticle = _itemRepository.UpdateItem(item);
                ViewBag.CategorieId = new SelectList(_commonService.GetCategories(), "CategorieId", "Nom");
                if (updatedArticle != null)
                    return RedirectToAction("listeproduit");
                else
                    return NotFound();

            }
            return View(model);
        }

        [NonAction]
        private string ProcessUploadedFile(EditItemModel model)
        {
            string uniqueFileName = null;
            if (model.ImagePath != null)
            {
                string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImagePath.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.ImagePath.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
        [HttpPost]
        public IActionResult EditItem(int id, Item item)
        {
            if (id != item.Id)
            {
                return BadRequest();
            }

            var existingItem = _commonService.GetItem(id);
            if (existingItem == null)
            {
                return NotFound();
            }

            _itemRepository.UpdateItem(item);
            return RedirectToAction(nameof(Items));
        }

        public ActionResult DeleteItem(int id)
        {
            var item = _commonService.GetItem(id);
            if (item == null)
            {
                return NotFound();
            }
            return View("Items/DeleteItem", item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteItemConfirmed(int id)
        {
            var item = _itemRepository.DeleteItem(id);
            if (item == null)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Items));
        }



        public IActionResult SearchByNameOrPrice(string nameOrPrice)
        {
            try
            {
                var items = _commonService.SearchByNameOrPrice(nameOrPrice);
                if (items.Count == 0)
                {
                    ViewBag.Message = "No items found.";
                }
                return View("items/SearchByNameOrPrice",items);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Internal server error: {ex.Message}";
                return View("items/SearchByNameOrPrice");
            }
        }

        public IActionResult SortItems(string sortBy)
        {
            try
            {
                var items = _commonService.SortItemsBy(sortBy);
                if (items == null)
                {
                    ViewBag.Message = "No items found.";
                }
                return View("items/SortItems",items);
            }
            catch (ArgumentException ex)
            {
                ViewBag.Error = ex.Message;
                return View("items/SortItems");
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Internal server error: {ex.Message}";
                return View("items/SortItems");
            }
        }

        public ActionResult<IEnumerable<Item>> GetItemsByCategory(int categoryId)
        {
            var items = _commonService.GetItemsByCategory(categoryId);
            if (items == null)
            {
                return View("Error");
            }
            return View(items);
        }

       

        public IActionResult IndexCategory()
        {
            var categories = _commonService.GetCategories();
            return View("categories/IndexCategory", categories);
        }

        public IActionResult DetailsCategory(int id)
        {
            var category = _commonService.GetCategory(id);
            if (category == null)
            {
                return NotFound();
            }

            var items = _commonService.GetItemsByCategory(id);

            var model = new CategoryDetailsViewModel
            {
                Category = category,
                Items = (List<Item>)items
            };

            return View("categories/DetailsCategory",model);
        }

        public IActionResult CreateCategory()
        {
            return View("categories/CreateCategory");
        }

        [HttpPost]
        public IActionResult CreateCategory(Category category)
        {
          
                _categoryRepository.AddCategory(category);
                return RedirectToAction(nameof(IndexCategory));
            
        
        }

        public IActionResult EditCategory(int id)
        {
            var category = _commonService.GetCategory(id);
            if (category == null)
            {
                return NotFound();
            }
            return View("categories/EditCategory", category);
        }

        [HttpPost]
        public IActionResult EditCategory(int id, Category category)

        {
            var categories = _commonService.GetCategories();
            if (id != category.Id)
            {
                return BadRequest();
            }

            
                var updatedCategory = _categoryRepository.UpdateCategory(category);
                if (updatedCategory == null)
                {
                    return NotFound();
                }

               
                return View("categories/IndexCategory", categories); 
                      
        }

        public IActionResult DeleteCategory(int id)
        {
            var category = _commonService.GetCategory(id);
            if (category == null)
            {
                return NotFound();
            }

            return View("categories/DeleteCategory",category);
        }

        // HTTP POST action to actually delete the category
        [HttpPost, ActionName("DeleteCategory")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var category = _categoryRepository.DeleteCategory(id);
            if (category == null)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(IndexCategory));
        }




    }

}