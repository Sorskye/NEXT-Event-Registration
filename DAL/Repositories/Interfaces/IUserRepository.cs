using DAL.Models;

namespace DAL.Repositories.Interfaces;

public interface IUserRepository // Verantwoordelijk voor [GEBRUIKERS] vinden bij login, en checken of iemand admin is
{
    User GetUserByEmail(string email);
    bool CreateUser(User user);
    bool IsUserAdmin(int userId);
    
    
    bool CreateUser(User user);
}
