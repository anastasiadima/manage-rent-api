using ManageRentApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageRentApi.Interfaces
{
     public interface ITenantRepository:  IRepository<Tenant, int>
     {
     }
}
