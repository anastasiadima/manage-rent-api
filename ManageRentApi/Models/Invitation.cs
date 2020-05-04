namespace ManageRentApi.Models
{
     public class Invitation
     {
          public int Id { get; set; }
          public int SenderId { get; set; }
          public string Status { get; set; }
          public string ReceiverEmail { get; set; }
          public string SenderEmail { get; set; }
     }
}
