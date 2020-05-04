using ManageRentApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ManageRentApi.Helpers
{
     public class DataContext: DbContext
     {
          public DataContext(DbContextOptions<DataContext> options) : base(options) { }

          public DbSet<User> Users { get; set; }
          public DbSet<House> Houses { get; set; }
          public DbSet<Tenant> Tenants { get; set; }
          public DbSet<Plan> Plans { get; set; }
          public DbSet<TenantPlan> TenantPlan { get; set; }
          public DbSet<OwnerTenant> OwnerTenants { get; set; }
          public DbSet<Invitation> Invitations { get; set; }
     }
}
