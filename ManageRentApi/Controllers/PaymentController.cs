using ManageRentApi.Dtos;
using ManageRentApi.Interfaces;
using ManageRentApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace ManageRentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController] 
     public class PaymentController : ControllerBase
    {
         private readonly IPaymentService _paymentService;
          private readonly IPlanRepository _planRepository;
          private readonly ITenantPlanRepository _tenantPlanRepository;
          private readonly ITenantRepository _tenantRepository;

         public PaymentController(IPaymentService paymentService, 
              IPlanRepository planRepository, ITenantPlanRepository tenantPlanRepository,
              ITenantRepository tenantRepository)
         {
              _paymentService = paymentService;
               _planRepository = planRepository;
               _tenantPlanRepository = tenantPlanRepository;
               _tenantRepository = tenantRepository;
         }

         
          [HttpPost("createplan")]
          public IActionResult CreatePlan([FromBody]PlanDto plan)
          {
               _paymentService.CreatePlan(plan);
               return Ok();
          }

          
          [HttpGet("list")]
          public IActionResult List()
          {
               var claimsIdentity = this.User.Identity as ClaimsIdentity;
               var userId = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
               bool succes = Int32.TryParse(userId, out var ownerId);
               if (succes)
               {
                    return Ok(_planRepository.GetAll().Where(e => e.OwnerId == ownerId));
               }
               return Ok(_planRepository.GetAll());
          }

          [HttpGet("plan/{id}")]
          public IActionResult GetPlan(int id)
          {
               var planDetails = _paymentService.GetPlan(id);
               
               return Ok(planDetails);
          }

          
          [HttpGet("subscribedUsers/{id}")]
          public IActionResult SubscribedUsersForPlan(int id)
          {
               List<TenantPlan> subscribedTenantIds = _tenantPlanRepository.GetAll().Where(entity => entity.PlanId == id).ToList();
               IList<SubscribedUser> subscribedUser = new List<SubscribedUser>();

               foreach (var tenantPlan in subscribedTenantIds)
               {
                    var details = _tenantRepository.GetById(tenantPlan.TenantId);
                    if (details != null)
                    {
                         subscribedUser.Add(new SubscribedUser() {
                         Id = details.Id,
                         FirstName = details.FirstName,
                         LastName = details.LastName,
                         PlanId = id,
                         Subscribed = true
                         });
                    }
               }
               var claimsIdentity = this.User.Identity as ClaimsIdentity;
               var userId = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
               bool succes = Int32.TryParse(userId, out var ownerId);
               if (succes)
               {
                    var tenants = _tenantRepository.GetAll().Where(t => t.OwnerId == ownerId).ToList();
                    foreach (var subscribedTenant in subscribedUser)
                    {
                         var tenant = tenants.Where(t => t.Id == subscribedTenant.Id).First();
                         tenants.Remove(tenant);
                    }

                    foreach (var t in tenants)
                    {
                         subscribedUser.Add(new SubscribedUser()
                         {
                              Id = t.Id,
                              FirstName = t.FirstName,
                              LastName = t.LastName,
                              PlanId = id,
                              Subscribed = false
                         });
                    }
               }

               return Ok(subscribedUser);
          }

        
          [HttpPost("subscribeUsers")]
          public ActionResult<string> Subscribe([FromBody]SubscribedUserList subscribeUserList)
         {
               var tenantPlanList = _tenantPlanRepository.GetAll().ToList();

               foreach (var subscribeUser in subscribeUserList.subscribedUsers)
               {
                    var planTenant = tenantPlanList.Where(t => t.PlanId == subscribeUser.PlanId && subscribeUser.Id == t.TenantId).FirstOrDefault();

                    if (subscribeUser.Subscribed)
                    {
                         if (planTenant == null)
                         {
                              _tenantPlanRepository.Insert(new TenantPlan {
                                   PlanId = subscribeUser.PlanId,
                                   TenantId = subscribeUser.Id
                              });
                         }
                    } else
                    {
                         if (planTenant != null)
                         {
                              _tenantPlanRepository.Delete(planTenant.Id);
                              _tenantPlanRepository.Save();
                         }
                    }
               }

              return Ok();
         }
     }
}