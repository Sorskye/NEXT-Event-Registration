using DAL.Models;

namespace DAL.Repositories.Interfaces;

public interface IUserRepository
{
    User GetUserByEmail(string email);
    bool IsUserAdmin(int userId);
}
