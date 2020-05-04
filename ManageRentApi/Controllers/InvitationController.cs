using ManageRentApi.Interfaces;
using ManageRentApi.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;

namespace ManageRentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvitationController : ControllerBase
    {
          private IInvitationRepository _invitiationRepository;
          private readonly IUserService _userService;

          public InvitationController(IInvitationRepository invitationRepository, IUserService userService)
          {
               _invitiationRepository = invitationRepository;
               _userService = userService;
          }

          [HttpGet]
          public IActionResult GetAll()
          {
               var invitations = _invitiationRepository.GetAll();
               var claimsIdentity = this.User.Identity as ClaimsIdentity;
               var user = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
               bool succes = Int32.TryParse(user, out var userId);

               if (succes)
               {
                    var loggedUser = _userService.GetById(userId);
                    if (loggedUser!= null)
                    {
                         return Ok(invitations.Where(h => h.ReceiverEmail == loggedUser.Email));
                    }
               }
               return Ok();
          }
          [HttpPost("create")]
          public IActionResult Create([FromBody]Invitation invitation)
          {
               invitation.Status = "CREATED";
               _invitiationRepository.Insert(invitation);
               _invitiationRepository.Save();
               return Ok();
          }
     }
}