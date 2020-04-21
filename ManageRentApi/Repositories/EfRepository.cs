using ManageRentApi.Helpers;
using ManageRentApi.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace ManageRentApi.Services
{
     public class EfRepository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : class
     {
          private readonly DataContext _context;

          public EfRepository(DataContext context)
          {
               _context = context; 
          }

          protected DbSet<TEntity> Set => _context.Set<TEntity>();

          public IEnumerable<TEntity> GetAll()
          {
               return Set.ToList();
          }

          public TEntity GetById(TKey id)
          {
               return Set.Find(id);
          }

          public TEntity Insert(TEntity entity)
          {
               var e = Set.Add(entity);
               Save();
               return e.Entity;
          }

          public void Delete(int id)
          {
               var entity = Set.Find(id);
               Set.Remove(entity);
          }

          public void Update(TEntity entity)
          {
               Set.Update(entity);
               Save();
          }

          public void Save()
          {
               _context.SaveChanges();
          }
     }
}
