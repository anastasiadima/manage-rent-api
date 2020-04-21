using ManageRentApi.Helpers;
using ManageRentApi.Interfaces;
using ManageRentApi.Models;

namespace ManageRentApi.Services
{
     public class PlanRepository: EfRepository<Plan, int>, IPlanRepository
     {
          public PlanRepository(DataContext context) : base(context) { }
     }
}
