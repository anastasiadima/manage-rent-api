namespace ManageRentApi.Dtos
{
     public class HouseDto
     {
          public int Id { get; set; }
          public string Name { get; set; }
          public string Address { get; set; }
          public string Description { get; set; }
          public int NumberOfRooms { get; set; }
          public int OwnerId { get; set; }
     }
}
