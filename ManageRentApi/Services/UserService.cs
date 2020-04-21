using ManageRentApi.Helpers;
using ManageRentApi.Interfaces;
using ManageRentApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ManageRentApi.Services
{
     public class UserService :  IUserService
     {
          private readonly DataContext _context;
          private readonly ITenantRepository _tenantRepository;

          public UserService(DataContext context, ITenantRepository tenantRepository)
          {
               _context = context;
               _tenantRepository = tenantRepository;
          }
          protected DbSet<User> Set => _context.Set<User>();

          public User Authenticate(string username, string password)
          {
               if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                    return null;
               var user = GetAll().ToList().SingleOrDefault(x => x.Username == username);

               // check if username exists
               if (user == null)
                    return null;

               // check if password is correct
               if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                    return null;

               // authentication successful
               return user;
          }
          
          private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
          {
               if (password == null) throw new ArgumentNullException("password");
               if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

               using (var hmac = new System.Security.Cryptography.HMACSHA512())
               {
                    passwordSalt = hmac.Key;
                    passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
               }
          }

          private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
          {
               if (password == null) throw new ArgumentNullException("password");
               if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
               if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
               if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

               using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
               {
                    var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                    for (int i = 0; i < computedHash.Length; i++)
                    {
                         if (computedHash[i] != storedHash[i]) return false;
                    }
               }

               return true;
          }

          public IEnumerable<User> GetAll()
          {
               return Set.ToList();
          }

          public User GetById(int id)
          {
               return Set.Find(id);
          }

          public User Insert(User user, string password)
          {
               if(string.IsNullOrWhiteSpace(password))
                throw new CustomException("Password is required");

               if (_context.Users.Any(x => x.Username == user.Username))
                    throw new CustomException("Username \"" + user.Username + "\" is already taken");

               byte[] passwordHash, passwordSalt;
               CreatePasswordHash(password, out passwordHash, out passwordSalt);

               user.PasswordHash = passwordHash;
               user.PasswordSalt = passwordSalt;

               Set.Add(user);
               Save();

               if (user.Role == Role.Tenant)
               {
                    var tenant = _tenantRepository.GetAll().Where(t => t.Email.Equals(user.Email)).FirstOrDefault();

                    if (tenant != null)
                    {
                         tenant.UserId = user.Id;
                         _tenantRepository.Update(tenant);
                         _tenantRepository.Save();
                    }
               }

               return user;
          }

          public void Delete(int id)
          {
               var user = Set.Find(id);
               Set.Remove(user);
          }

          public void UpdatePassword(User userParam, string password = null)
          {
               var user = _context.Users.Find(userParam.Id);

               if (user == null)
                    throw new CustomException("User not found");

               if (userParam.Username != user.Username)
               {
                    // username has changed so check if the new username is already taken
                    if (_context.Users.Any(x => x.Username == userParam.Username))
                         throw new CustomException("Username " + userParam.Username + " is already taken");
               }

               // update user properties
               user.FirstName = userParam.FirstName;
               user.LastName = userParam.LastName;
               user.Username = userParam.Username;

               // update password if it was entered
               if (!string.IsNullOrWhiteSpace(password))
               {
                    byte[] passwordHash, passwordSalt;
                    CreatePasswordHash(password, out passwordHash, out passwordSalt);

                    user.PasswordHash = passwordHash;
                    user.PasswordSalt = passwordSalt;
               }

               Set.Update(user);
               Save();
          }

          public void Save()
          {
               _context.SaveChanges();
          }

          public User Insert(User entity)
          {
               throw new NotImplementedException();
          }

          public void Update(User entity)
          {
               Set.Update(entity);
               Save();
          }
     }
}
