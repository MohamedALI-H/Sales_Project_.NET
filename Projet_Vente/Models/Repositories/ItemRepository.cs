using Microsoft.EntityFrameworkCore;
using Projet_Vente.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Projet_Vente.Models.Repositories
{
    public class ItemRepository : IItemRepository
    {
        private readonly AppDbContext _appDbContext;

        public ItemRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public IEnumerable<Item> GetItems()
        {
            return _appDbContext.Items.ToList();
        }

        public Item GetItem(int id)
        {
            return _appDbContext.Items.Find(id);
        }

        public Item AddItem(Item item)
        {
            _appDbContext.Items.Add(item);
            _appDbContext.SaveChanges();
            return item;
        }

        public Item UpdateItem(Item item)
        {
            var existingItem = _appDbContext.Items.Find(item.Id);
            if (existingItem != null)
            {
                existingItem.Name = item.Name;
                existingItem.Description = item.Description;
                existingItem.Price = item.Price;
                existingItem.ImageUrl = item.ImageUrl;
                existingItem.CategoryId = item.CategoryId;

                _appDbContext.SaveChanges();
                return existingItem;
            }
            return null;
        }

        public Item DeleteItem(int id)
        {
            var item = _appDbContext.Items.Find(id);
            if (item != null)
            {
                _appDbContext.Items.Remove(item);
                _appDbContext.SaveChanges();
                return item;
            }
            return null;
        }
    }
}
