
namespace Projet_Vente.Models.Repositories
{
    public interface ICommonService
    {
        IEnumerable<Category> GetCategories();
        Category GetCategory(int id);
        IEnumerable<Item> GetItems();
        Item GetItem(int id);
        List<Item> SearchByNameOrPrice(string nameOrPrice);
        IEnumerable<Item> GetItemsByCategory(int categoryId);
        IEnumerable<Item> SortItemsBy(string sortBy);
    }
}
