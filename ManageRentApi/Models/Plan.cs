namespace ManageRentApi.Models
{
     public class Plan
     {
          public int Id { get; set; }
          public string Name { get; set; }
          public string PlanId { get; set; }
          public int OwnerId { get; set; }
          public int TenantId { get; set; }
          public bool IsSubscribed { get; set; }
     }
}
