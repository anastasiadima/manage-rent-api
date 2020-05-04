namespace ManageRentApi.Models
{
     public class OwnerTenant
     {
          public int Id { get; set; }
          public int TenantId { get; set; }
          public int OwnerId { get; set; }
          public string Status { get; set; }
     }
}
