using AutoMapper;
using ManageRentApi.Dtos;
using ManageRentApi.Helpers;
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
     [Authorize]
     [ApiController]
     [Route("api/[controller]")]

     public class TenantsController: ControllerBase
     {
          private ITenantRepository _tenantRepository;
          private IHouseRepository _houseRepository;
          private IMapper _mapper;
          private readonly ITenantService _tenantService;

          public TenantsController(
              ITenantRepository tenantRepository,
              IMapper mapper,
              IHouseRepository houseRepository,
              ITenantService tenantService)
          {
               _tenantRepository = tenantRepository;
               _mapper = mapper;
               _houseRepository = houseRepository;
               _tenantService = tenantService;
          }

          [HttpGet]
          public IActionResult GetAll()
          {
               var claimsIdentity = this.User.Identity as ClaimsIdentity;
               var userId = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
               bool succes = Int32.TryParse(userId, out var ownerid);
               IList<Tenant> tenants;
               if (succes)
               {
                    tenants = _tenantRepository.GetAll().Where(t => t.OwnerId == ownerid).ToList();
                    return Ok(tenants);
               }
               tenants = _tenantRepository.GetAll().ToList();
               return Ok(tenants);
          }

          [HttpGet("{id}")]
          public IActionResult GetById(int id)
          {
               var tenant = _tenantRepository.GetById(id);
               return Ok(tenant);
          }

          [HttpPut("{id}")]
          public IActionResult Update(int id, [FromBody]Tenant tenant)
          {
               // map dto to entity and set id
               var user = _mapper.Map<Tenant>(tenant);
               user.Id = id;

               try
               {
                    // save 
                    _tenantRepository.Update(tenant);
                    _tenantRepository.Save();
                    return Ok();
               }
               catch (CustomException ex)
               {
                    // return error message if there was an exception
                    return BadRequest(new { message = ex.Message });
               }
          }
          [HttpDelete("{id}")]
          public IActionResult Delete(int id)
          {
               _tenantRepository.Delete(id);
               _tenantRepository.Save();
               return Ok();
          }

          [HttpPost("create")]
          public IActionResult Create([FromBody]TenantDto tenanDto)
          {
               var tenant = _mapper.Map<Tenant>(tenanDto);
               var house = _houseRepository.GetById(tenanDto.HouseId);

               var claimsIdentity = this.User.Identity as ClaimsIdentity;
               var userId = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
               bool succes = Int32.TryParse(userId, out var ownerId);
               if (succes)
               {
                    tenant.OwnerId = ownerId;
               }
               if (house != null)
               {
                    tenant.House = house;
               }

               _tenantRepository.Insert(tenant);
               _tenantRepository.Save();

               return Ok();
          }

          [HttpGet("planList")]
          public IActionResult PlanList()
          {
               var claimsIdentity = this.User.Identity as ClaimsIdentity;
               var userIdString = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
               bool succes = Int32.TryParse(userIdString, out var userId);
               if (succes)
               {
                    var tenantid = _tenantService.GetTenantIdForUser(userId);
                    var plans = _tenantService.GetPlansForTenant(tenantid);
                    return Ok(plans);
               }
               
               return Ok();
          }
     }
}
