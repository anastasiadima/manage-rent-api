using ManageRentApi.Helpers;
using ManageRentApi.Interfaces;
using ManageRentApi.Models;
using ManageRentApi.Services;

namespace ManageRentApi.Repositories
{
     public class OwnerTenantRepository: EfRepository<OwnerTenant, int>, IOwnerTenantRepository
     {
          public OwnerTenantRepository(DataContext data): base(data) { }
     }
}
