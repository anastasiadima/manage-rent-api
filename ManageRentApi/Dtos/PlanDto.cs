namespace ManageRentApi.Dtos
{
     public class PlanDto
     {
          public string Name { get; set; }
          public string Description { get; set; }
          public string Type { get; set; }
          public string PaymentName { get; set; }
          public string PaymentType { get; set; }
          public string PaymentFrequency { get; set; }
          public string PaymentFrequencyInterval { get; set; }
          public string PaymentCycles { get; set; }
          public string PaymentAmount { get; set; }
          public string PaymentCurrency { get; set; }
          public int OwnerId { get; set; }
     }
}
