using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Projet_Vente.Models.Repositories
{
    public class CommonService : ICommonService
    {
        private readonly AppDbContext _appDbContext;

        public CommonService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public IEnumerable<Category> GetCategories()
        {
            return _appDbContext.Categories.ToList();
        }

        public Category GetCategory(int id)
        {
            return _appDbContext.Categories.Find(id);
        }

        public IEnumerable<Item> GetItems()
        {
            return _appDbContext.Items.Include(i => i.Category).ToList();
        }

        public Item GetItem(int id)
        {
            return _appDbContext.Items.Include(i => i.Category).FirstOrDefault(i => i.Id == id);

        }

        public List<Item> SearchByNameOrPrice(string nameOrPrice)
        {
            if (decimal.TryParse(nameOrPrice, out decimal price))
            {
                return _appDbContext.Items
                                    .Where(i => i.Price == price)
                                    .Include(o => o.Category)
                                    .ToList();
            }
            else
            {
                return _appDbContext.Items
                                    .Where(i => i.Name.Contains(nameOrPrice))
                                    .Include(o => o.Category)
                                    .ToList();
            }
        }

        public IEnumerable<Item> GetItemsByCategory(int categoryId)
        {
            return _appDbContext.Items.Where(i => i.CategoryId == categoryId).ToList();
        }

        public IEnumerable<Item> SortItemsBy(string sortBy)
        {
            IQueryable<Item> query = _appDbContext.Items.Include(o => o.Category);

            switch (sortBy.ToLower())
            {
                case "descending":
                    query = query.OrderByDescending(i => i.Price);
                    break;
                case "ascending":
                    query = query.OrderBy(i => i.Price);
                    break;
                default:
                    throw new ArgumentException("Invalid sortBy parameter.");
            }


            return query.ToList();
        }
    }
}
