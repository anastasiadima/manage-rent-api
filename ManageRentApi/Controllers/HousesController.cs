using AutoMapper;
using ManageRentApi.Dtos;
using ManageRentApi.Helpers;
using ManageRentApi.Interfaces;
using ManageRentApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;

namespace ManageRentApi.Controllers
{
     [Authorize]
     [ApiController]
     [Route("api/[controller]")]
     public class HousesController : Controller
     {
          private IHouseRepository _houseRepository;
          private IUserService _userService;
          private IMapper _mapper;

          public HousesController(
              IHouseRepository houseRepository, IUserService userService,
              IMapper mapper)
          {
               _houseRepository = houseRepository;
               _mapper = mapper;
               _userService = userService;
          }

          [HttpGet]
          public IActionResult GetAll()
          {
               var houses = _houseRepository.GetAll();
               var claimsIdentity = this.User.Identity as ClaimsIdentity;
               var userId = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
               bool succes = Int32.TryParse(userId, out var ownerId);
               if (succes)
               {
                    return Ok(houses.Where(h => h.OwnerId == ownerId));
               }
               return Ok(houses);
          }

          [HttpGet("{id}")]
          public IActionResult GetById(int id)
          {
               var house = _houseRepository.GetById(id);
               return Ok(house);
          }

          [HttpPut("{id}")]
          public IActionResult Update(int id, [FromBody]House house)
          {
               try
               {
                    // save 
                    _houseRepository.Update(house);
                    _houseRepository.Save();
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
               _houseRepository.Delete(id);
               _houseRepository.Save();
               return Ok();
          }

          [HttpPost("create")]
          public IActionResult Create([FromBody]HouseDto houseDto)
          {
               House house = _mapper.Map<House>(houseDto);
               var claimsIdentity = this.User.Identity as ClaimsIdentity;
               var userId = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
               bool succes = Int32.TryParse(userId, out var ownerId);
               if (succes)
               {
                    house.OwnerId = ownerId;
               }
               _houseRepository.Insert(house);
               _houseRepository.Save();
               return Ok();
          }
     }
}
