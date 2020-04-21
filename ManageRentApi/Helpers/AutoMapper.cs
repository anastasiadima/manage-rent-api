using AutoMapper;
using ManageRentApi.Dtos;
using ManageRentApi.Models;

namespace ManageRentApi.Helpers
{
     public class AutoMapper: Profile
     {
         public AutoMapper()
          {
               CreateMap<User, UserDto>();
               CreateMap<UserDto, User>();
               CreateMap<House, HouseDto>();
               CreateMap<HouseDto, House>();
               CreateMap<Tenant, TenantDto>();
               CreateMap<TenantDto, Tenant>();
          }
     }
}
