using ManageRentApi.Dtos;
using ManageRentApi.Interfaces;
using ManageRentApi.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ManageRentApiUnitTest
{
     [TestClass]
     public class PlanTest
     {
          IPaymentService _paymentService;
          IPlanRepository _planRepository;
          ITenantPlanRepository _tenantPlanRepository;

          [TestInitialize]
          public void Initialize()
          {
               _tenantPlanRepository = Mock.Of<ITenantPlanRepository>();
               _planRepository = Mock.Of<IPlanRepository>();
               _paymentService = new PaymentService(_planRepository, _tenantPlanRepository);
          }


          [TestMethod]
          public void CreatesPlan_ShouldBeGetedPlan()
          {
               var planDto = new PlanDto() {
                    Name = "",
                    PaymentAmount = "20",
                    PaymentCycles = "1",
                    PaymentFrequency = "1",
                    PaymentCurrency = "USD",
                    PaymentFrequencyInterval = "monthly",
                    OwnerId = 1,
                    Description = "Description"
               };

               var idPlan = _paymentService.CreatePlan(planDto);

               var createdPlan = _planRepository.GetById(idPlan);
               //Assert.Equals(planDto.Name, createdPlan.Name);
          }
     }
}
