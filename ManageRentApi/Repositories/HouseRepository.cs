using ManageRentApi.Helpers;
using ManageRentApi.Interfaces;
using ManageRentApi.Models;

namespace ManageRentApi.Services
{
     public class HouseRepository: EfRepository<House, int>, IHouseRepository
     {
          public HouseRepository(DataContext context): base(context) { }
     }
}
