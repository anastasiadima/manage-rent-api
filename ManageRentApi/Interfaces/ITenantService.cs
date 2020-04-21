using ManageRentApi.Models;
using System.Collections.Generic;

namespace ManageRentApi.Interfaces
{
     public interface ITenantService
     {
          void Update(Tenant tenant);
          IList<PayPal.Api.Plan> GetPlansForTenant(int id);
          int GetTenantIdForUser(int id);
     }
}
