
using ManageRentApi.Dtos;
using PayPal.Api;

namespace ManageRentApi.Interfaces
{
     public interface IPaymentService
     { 
          int CreatePlan(PlanDto plan);
          void SubscribeToPlan(string planId, int tenantId);
          PayPal.Api.Plan GetPlan(int id);
          void Subscribe();
     }
}
