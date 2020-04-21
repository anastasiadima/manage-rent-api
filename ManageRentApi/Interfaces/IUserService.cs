using ManageRentApi.Models;

namespace ManageRentApi.Interfaces
{
     public interface IUserService: IRepository<User, int>
     {
          User Authenticate(string username, string password);
          void UpdatePassword(User user, string password);
          User Insert(User user, string password);
     }
}
