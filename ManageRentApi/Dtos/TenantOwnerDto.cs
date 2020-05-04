namespace ManageRentApi.Dtos
{
     public class TenantOwnerDto
     {
          public int Id { get; set; }
          public int HouseId { get; set; }
          public int OwnerId { get; set; }
          public string LastName { get; set; }
          public string FirstName { get; set; }
          public string Email { get; set; }
          public int MyProperty { get; set; }
     }
}
