using DAL.Models;
using DAL.Repositories.Interfaces;

namespace NEXT.Tests.Fakes
{
    public class FakeUserRepository : IUserRepository
    {
        public User UserToReturn { get; set; }
        public bool IsAdminToReturn { get; set; }

        public User GetUserByEmail(string email)
        {
            return UserToReturn;
        }

        public bool IsUserAdmin(int userId)
        {
            return IsAdminToReturn;
        }

        public bool CreateUser(User user)
        {
            throw new NotImplementedException();
        }
    }
}