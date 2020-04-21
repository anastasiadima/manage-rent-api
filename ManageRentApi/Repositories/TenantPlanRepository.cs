using ManageRentApi.Helpers;
using ManageRentApi.Interfaces;
using ManageRentApi.Models;

namespace ManageRentApi.Services
{
     public class TenantPlanRepository : EfRepository<TenantPlan, int>, ITenantPlanRepository
     {
          public TenantPlanRepository(DataContext dataContext) : base(dataContext)
          {
          }
     }
}
