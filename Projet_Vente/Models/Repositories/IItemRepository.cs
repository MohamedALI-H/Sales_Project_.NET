using Projet_Vente.Models;
using System.Collections.Generic;

namespace Projet_Vente.Models.Repositories
{
    public interface IItemRepository
    {
        IEnumerable<Item> GetItems();
        Item GetItem(int id);
        Item AddItem(Item item);
        Item UpdateItem(Item item);
        Item DeleteItem(int id);
    }
}
