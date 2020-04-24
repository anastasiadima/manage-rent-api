using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using ManageRentApi.Dtos;
using ManageRentApi.Interfaces;
using ManageRentApi.Models;
using PayPal.Api;

namespace ManageRentApi.Services
{
     public class PaymentService: IPaymentService
     {
          private APIContext _apiContext;
          private object shippingChargeModel;
          private readonly IPlanRepository _planRepository;
          private readonly ITenantPlanRepository _tenantPlanRepository;

          public PaymentService(IPlanRepository planRepository, ITenantPlanRepository tenantPlanRepository)
          {
               this._planRepository = planRepository;
               this._tenantPlanRepository = tenantPlanRepository;
               var config = new Dictionary<string, string>();
               var mode = ConfigurationManager.AppSettings["mode"];
               var clientId = ConfigurationManager.AppSettings["clientId"];
               var clientSecret = ConfigurationManager.AppSettings["clientSecret"];
               config.Add("mode", mode);
               config.Add("clientId", clientId);
               config.Add("clientSecret", clientSecret);

               // Use OAuthTokenCredential to request an access token from PayPal
               var accessToken = new OAuthTokenCredential(config).GetAccessToken();
               _apiContext = new APIContext(accessToken);
               // Initialize the apiContext's configuration with the default configuration for this application.
               _apiContext.Config = ConfigManager.Instance.GetProperties();

               // Define any custom configuration settings for calls that will use this object.
               _apiContext.Config["connectionTimeout"] = "3000"; // Quick timeout for testing purposes

               // Define any HTTP headers to be used in HTTP requests made with this APIContext object
               if (_apiContext.HTTPHeaders == null)
               {
                    _apiContext.HTTPHeaders = new Dictionary<string, string>();
               }
               _apiContext.HTTPHeaders["some-header-name"] = "some-value";
               
          }

          public int CreatePlan(PlanDto planDto)
          { 
               var currency = new Currency()
               {
                    value = planDto.PaymentAmount,
                    currency = planDto.PaymentCurrency
               };

               var shippingChargeModel = new ChargeModel()
               {
                    type = "SHIPPING",
                    amount = new Currency()
                    {
                         value = "1",
                         currency = planDto.PaymentCurrency
                    }
               };

               var paymentDefinition = new PaymentDefinition()
               {
                    name = planDto.PaymentName,
                    type = planDto.PaymentType,
                    frequency = planDto.PaymentFrequency,
                    frequency_interval = planDto.PaymentFrequencyInterval,
                    amount = currency,
                    cycles = planDto.PaymentCycles,
                    charge_models = new List<ChargeModel>
                        {
                            new ChargeModel()
                            {
                                type = "TAX",
                                amount = new Currency{
                                     value = "1",
                                     currency = planDto.PaymentCurrency
                                }
                            },
                            shippingChargeModel
                        }
               };

               var plan = new PayPal.Api.Plan
               {
                    name = planDto.Name,
                    description = planDto.Description,
                    type = "fixed",

                    payment_definitions = new List<PaymentDefinition>
                    {
                         paymentDefinition
                    },

                    merchant_preferences = new MerchantPreferences()
                    {
                         setup_fee = new Currency()
                         {
                              value = "0",
                              currency = planDto.PaymentCurrency
                         },
                         return_url = "https://localhost:5001/api/values",
                         cancel_url = "https://localhost:5001/api/houses",
                         auto_bill_amount = "YES",
                         initial_fail_amount_action = "CONTINUE",
                         max_fail_attempts = "0"
                    }
               };

               var createdPlan = plan.Create(_apiContext);
               var state = new
               {
                    state = "ACTIVE"
               };
               var patchRequest = new PatchRequest()
               {
                    new Patch()
                    {
                         op = "replace",
                         path = "/",
                         value = state
                    }
               };
               createdPlan.Update(_apiContext, patchRequest);

               //save plan in db
               var planToSave = new Models.Plan
               {
                    Name = createdPlan.name,
                    PlanId = createdPlan.id,
                    OwnerId = planDto.OwnerId
               };

               var planInsert = _planRepository.Insert(planToSave);
               return planInsert.Id;
          } 

          public void SubscribeToPlan(string planId, int tenantId)
          {
               Models.Plan plan = _planRepository.GetAll().Where(p => p.PlanId == planId).FirstOrDefault();
               plan.TenantId = tenantId;
               _planRepository.Update(plan);
          }

          public PayPal.Api.Plan GetPlan(int id)
          {
               var plan = _planRepository.GetById(id);
               var planDetails = PayPal.Api.Plan.Get(_apiContext, plan.PlanId);

               return planDetails;
          }

          public IList<TenantPlan> GetSubribedUsersId(int planid)
          {
               return _tenantPlanRepository.GetAll().Where(en => en.PlanId == planid).ToList();
          }

          public void Subscribe()
          {
               throw new System.NotImplementedException();
          } 
     }
}
