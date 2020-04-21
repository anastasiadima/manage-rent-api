
namespace ManageRentApi.Models
{
     public class Tenant
     {
          public int Id { get; set; }
          public string Email { get; set; }
          public House House { get; set; }
          public string FirstName { get; set; }
          public string LastName { get; set; }
          public int OwnerId { get; set; }
          public int UserId { get; set; }
     }
}
