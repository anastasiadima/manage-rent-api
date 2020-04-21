using System.Collections.Generic;
using System.Linq;
using ManageRentApi.Interfaces;
using ManageRentApi.Models;

namespace ManageRentApi.Services
{
     public class TenantService: ITenantService
     {
          private readonly ITenantRepository _tenantRepository;
          private readonly IPlanRepository _planRepository;
          private readonly ITenantPlanRepository _tenantPlanRepository;
          private readonly IPaymentService _paymentService;

          public TenantService(ITenantRepository tenantRepository, 
               IPlanRepository planRepository, 
               ITenantPlanRepository tenantPlanRepository,
               IPaymentService paymentService)
          {
               _tenantRepository = tenantRepository;
               _planRepository = planRepository;
               _tenantPlanRepository = tenantPlanRepository;
               _paymentService = paymentService;
          }

          public IList<PayPal.Api.Plan> GetPlansForTenant(int id)
          {
               IList<PayPal.Api.Plan> plans = new List<PayPal.Api.Plan>();
               var listOfPlanIds = _tenantPlanRepository.GetAll().Where(entity => entity.TenantId == id).Select(e => e.PlanId).ToList();
               foreach (var planId in listOfPlanIds) {
                    var plan = _paymentService.GetPlan(planId);
                    if (plan != null)
                    {
                         plans.Add(plan);
                    }
               }

               return plans;
          }

          public int GetTenantIdForUser(int id)
          {
               var tenant = _tenantRepository.GetAll().Where(t => t.UserId == id).FirstOrDefault();
               if (tenant != null)
               {
                    return tenant.Id;
               }
               return 0;
          }

          public void Update(Tenant tenant)
          {
               _tenantRepository.Update(tenant);
               _tenantRepository.Save();
               return;
          }
     }
}
