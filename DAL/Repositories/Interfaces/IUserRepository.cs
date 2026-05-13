using DAL.Models;

namespace DAL.Repositories.Interfaces;

public interface IUserRepository
{
    User GetUserByEmailAndPassword(string email, string password);
    bool IsUserAdmin(int userId);
}
