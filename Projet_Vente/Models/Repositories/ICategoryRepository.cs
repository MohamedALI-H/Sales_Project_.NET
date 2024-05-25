using Projet_Vente.Models;

namespace Projet_Vente.Models.Repositories
{
    public interface ICategoryRepository
    {
        Category AddCategory(Category category);
        Category UpdateCategory(Category category);
        Category DeleteCategory(int id);
    }
}
