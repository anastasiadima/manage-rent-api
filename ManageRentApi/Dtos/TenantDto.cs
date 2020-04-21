namespace ManageRentApi.Dtos
{
     public class TenantDto
     {
          public int Id { get; set; }
          public int HouseId { get; set; }
          public int OwnerId { get; set; }
          public string LastName { get; set; }
          public string FirstName { get; set; }
          public string Email { get; set; }
     }
}
