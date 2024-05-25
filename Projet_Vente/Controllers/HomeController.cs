using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

using Projet_Vente.Models;
using Projet_Vente.Models.Repositories;

namespace Projet_Vente.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IItemRepository _itemRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICommonService _commonService;

    public HomeController(ILogger<HomeController> logger, IItemRepository itemRepository, ICategoryRepository categoryRepositor,
         ICommonService commonService)
    {
        _logger = logger;
        this._itemRepository = itemRepository;
        this._categoryRepository = categoryRepositor;
        this._commonService= commonService;
    }

    public IActionResult Index()
    {
        
        return View(_commonService.GetItems());
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }



    public IActionResult DetailsItem(int id)
    {
        var item = _commonService.GetItem(id);
        if (item == null)
        {
            return NotFound();
        }
        return View(item);
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
            return View( items);
        }
        catch (Exception ex)
        {
            ViewBag.Error = $"Internal server error: {ex.Message}";
            return View();
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
            return View(items);
        }
        catch (ArgumentException ex)
        {
            ViewBag.Error = ex.Message;
            return View();
        }
        catch (Exception ex)
        {
            ViewBag.Error = $"Internal server error: {ex.Message}";
            return View();
        }
    }

    public ActionResult Categories()
    {
        var categories = _commonService.GetCategories();
        return View(categories);
    }

    public IActionResult CategoryItems(int id)
    {
        var category =  _commonService.GetCategory(id);
        if (category == null)
        {
            return NotFound();
        }

        var items =  _commonService.GetItemsByCategory(id);

        var model = new CategoryDetailsViewModel
        {
            Category = category,
            Items = (List<Item>)items
        };

        return View(model);
    }



}

