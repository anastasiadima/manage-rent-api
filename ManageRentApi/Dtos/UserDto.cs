using ManageRentApi.Models;

namespace ManageRentApi.Dtos
{
     public class UserDto
     {
          public int Id { get; set; }
          public string FirstName { get; set; }
          public string LastName { get; set; }
          public string Username { get; set; }
          public string Password { get; set; }
          public string Email { get; set; }
          public string Role { get; set; }

          public void MapRole(User user)
          {
               if (user.Role == Models.Role.Owner)
               {
                    this.Role = "Owner";
               }

               if (user.Role == Models.Role.Tenant)
               {
                    Role = "Tenant";
               }
          }
     }

}
