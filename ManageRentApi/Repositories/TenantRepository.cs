using ManageRentApi.Helpers;
using ManageRentApi.Interfaces;
using ManageRentApi.Models;

namespace ManageRentApi.Services
{
     public class TenantRepository : EfRepository<Tenant, int>, ITenantRepository
     { 
          public TenantRepository(DataContext dataContext): base(dataContext)
          { 
          }
     }
}
