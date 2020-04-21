using System.Collections.Generic;

namespace ManageRentApi.Dtos
{
     public class SubscribedUser
     {
          public int Id { get; set; }
          public string FirstName { get; set; }
          public string LastName { get; set; }
          public int PlanId { get; set; }
          public bool Subscribed { get; set; }
     }
     public class SubscribedUserList
     {
          public List<SubscribedUser> subscribedUsers { get; set; }
     }
}
