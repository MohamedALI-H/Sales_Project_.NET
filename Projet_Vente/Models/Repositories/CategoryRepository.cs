using System.Linq;
using Projet_Vente.Models;

namespace Projet_Vente.Models.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _appDbContext;

        public CategoryRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public Category AddCategory(Category category)
        {
            var result = _appDbContext.Categories.Add(category);
            _appDbContext.SaveChanges();
            return result.Entity;
        }

        public Category UpdateCategory(Category category)
        {
            var existingCategory = _appDbContext.Categories.FirstOrDefault(c => c.Id == category.Id);
            if (existingCategory != null)
            {
                existingCategory.Name = category.Name;
                _appDbContext.SaveChanges();
                return existingCategory;
            }
            return null;
        }

        public Category DeleteCategory(int id)
        {
            var category = _appDbContext.Categories.FirstOrDefault(c => c.Id == id);
            if (category != null)
            {
                _appDbContext.Categories.Remove(category);
                _appDbContext.SaveChanges();
                return category;
            }
            return null;
        }
    }
}
