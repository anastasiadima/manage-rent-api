using AutoMapper;
using ManageRentApi.Dtos;
using ManageRentApi.Helpers;
using ManageRentApi.Interfaces;
using ManageRentApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ManageRentApi.Controllers
{
     [Authorize]
     [ApiController]
     [Route("api/[controller]")]

     public class UsersController : ControllerBase
     {
          private readonly IUserService _userService;
          private readonly IMapper _mapper;
          private readonly Setting _appSettings;
          private readonly IMailService _mailService;

          public UsersController(
              IUserService userService,
              IMapper mapper,
              IOptions<Setting> appSettings,
              IMailService mailService)
          {
               _userService = userService;
               _mapper = mapper;
               _appSettings = appSettings.Value;
               _mailService = mailService;
          }

          [AllowAnonymous]
          [HttpPost("register")]
          public IActionResult Register([FromBody]UserDto userDto)
          {
               // map dto to entity
               var user = _mapper.Map<User>(userDto);
               if (userDto.Role == "Owner")
               {
                    user.Role = Role.Owner;
               }
               if (userDto.Role == "Tenant") user.Role = Role.Tenant;

               try
               {
                    // save 
                    _userService.Insert(user, userDto.Password);

                    return Ok();
               }
               catch (CustomException ex)
               {
                    // return error message if there was an exception
                    return BadRequest(new { message = ex.Message });
               }
          }

          [AllowAnonymous]
          [HttpPost("authenticate")]
          public IActionResult Authenticate([FromBody]UserDto userDto)
          {
               var user = _userService.Authenticate(userDto.Username, userDto.Password);

               if (user == null)
                    return BadRequest(new { message = "Username or password is incorrect" });

               var tokenHandler = new JwtSecurityTokenHandler();
               var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
               var tokenDescriptor = new SecurityTokenDescriptor
               {
                    Subject = new ClaimsIdentity(new Claim[]
                   {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                   }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
               };
               var token = tokenHandler.CreateToken(tokenDescriptor);
               var tokenString = tokenHandler.WriteToken(token);
               string role = "";
               if (user.Role == Role.Owner)
               {
                    role = "Owner";
               }
               if (user.Role == Role.Tenant)
               {
                    role = "Tenant";
               }

               // return basic user info (without password) and token to store client side
               return Ok(new
               {
                    Id = user.Id,
                    Username = user.Username,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Role = role,
                    Token = tokenString
               });
          }

          [HttpGet]
          public IActionResult GetAll()
          {
               var users = _userService.GetAll();
               var userDtos = _mapper.Map<IList<UserDto>>(users);
               return Ok(userDtos);
          }

          [HttpGet("{id}")]
          public IActionResult GetById(int id)
          {
               var user = _userService.GetById(id);
               var userDto = _mapper.Map<UserDto>(user);
               userDto.MapRole(user);
               return Ok(userDto);
          }

          [HttpPut("{id}")]
          public IActionResult Update(int id, [FromBody]UserDto userDto)
          {
               // map dto to entity and set id
               var user = _mapper.Map<User>(userDto);
               user.Id = id;

               try
               {
                    // save 
                    _userService.UpdatePassword(user, userDto.Password);
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
               _userService.Delete(id);
               _userService.Save();
               return Ok();
          }

          [HttpPost("invite")]
          public IActionResult Invite([FromBody]EmailDtoWrapper email)
          {
               _mailService.SendInvitationEmail(email.Email);
               return Ok();
          }
     }
}
