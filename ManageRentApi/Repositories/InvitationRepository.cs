using ManageRentApi.Helpers;
using ManageRentApi.Interfaces;
using ManageRentApi.Models;
using ManageRentApi.Services;

namespace ManageRentApi.Repositories
{
     public class InvitationRepository: EfRepository<Invitation, int>, IInvitationRepository
     {
          public InvitationRepository(DataContext dataContext) : base(dataContext) { }
     }
}
