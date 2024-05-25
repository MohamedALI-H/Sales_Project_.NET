using Projet_Vente.Models;
using System.Collections.Generic;

namespace Projet_Vente.Models.Repositories
{
    public interface IUserRepository
    {
        IEnumerable<User> GetUsers();
        User GetUser(int userId);
        User AddUser(User user);
        User UpdateUser(User user);
        User DeleteUser(int userId);
        User SearchByName(string name);
    }
}
