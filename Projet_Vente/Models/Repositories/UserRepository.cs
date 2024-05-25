using System.Linq;

namespace Projet_Vente.Models.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _appDbContext;

        public UserRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public IEnumerable<User> GetUsers()
        {
            return _appDbContext.Users.ToList();
        }

        public User GetUser(int userId)
        {
            return _appDbContext.Users.FirstOrDefault(c => c.Id == userId);
        }

        public User AddUser(User user)
        {
            var result = _appDbContext.Users.Add(user);
            _appDbContext.SaveChanges();
            return result.Entity;
        }

        public User UpdateUser(User user)
        {
            var result = _appDbContext.Users.FirstOrDefault(c => c.Id == user.Id);
            if (result != null)
            {
                result.Name = user.Name;
                result.Email = user.Email;
                result.Password = user.Password;
                _appDbContext.SaveChanges();
                return result;
            }
            return null;
        }

        public User DeleteUser(int userId)
        {
            var result = _appDbContext.Users.FirstOrDefault(c => c.Id == userId);
            if (result != null)
            {
                _appDbContext.Users.Remove(result);
                _appDbContext.SaveChanges();
                return result;
            }
            return null;
        }

        public User SearchByName(string name)
        {
            return _appDbContext.Users.FirstOrDefault(c => c.Name == name);
        }
    }
}
