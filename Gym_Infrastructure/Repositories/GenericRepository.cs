using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Gym_Core.Interfaces;
using Gym_Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Gym_Infrastructure.Repositories
{
    public class GenericRepository<T> : IGeniricRepository<T> where T : class
    {
        private readonly GymDbContext db;

        public GenericRepository(GymDbContext db)
        {
            this.db = db;
        }
        public async Task<T> AddAsync(T Item)
        {
            if(Item == null)    
                throw new Exception("The item is not found");
                
            await db.Set<T>().AddAsync(Item);
            return Item;
        }

        public async Task DeleteById(int Id)
        {
            var Item = await db.Set<T>().FindAsync(Id);

            if (Item == null)
                throw new Exception($"The item is not found in {typeof(T).Name}"); 

            db.Set<T>().Remove(Item);
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> exp = null, string[] including = null)
        {
            IQueryable<T> query = db.Set<T>();
            if (including != null)
                foreach (var include in including)
                    query = query.Include(include);

            if (exp != null)
                return await query.Where(exp).ToListAsync();

            return await query.ToListAsync();
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> exp, string[] including = null)
        {
            IQueryable<T> query = db.Set<T>();
            if (including != null)
                foreach (var include in including)
                    query = query.Include(include);

            if (exp == null)
                return null!;

            return (await query.FirstOrDefaultAsync(exp))!;
        }

        public void Update(T Item)
        {
            if (Item == null)
                throw new Exception($"The item is not found in {typeof(T).Name}");


            db.Set<T>().Update(Item);
        }

        
    }
}
