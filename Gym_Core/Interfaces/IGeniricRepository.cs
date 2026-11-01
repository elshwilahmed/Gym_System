using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Gym_Core.Interfaces
{
    public interface IGeniricRepository<T> where T : class
    {
        Task<T> AddAsync(T Item);
        Task<T> GetAsync(Expression<Func<T, bool>> exp, string[] including = null!);
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> exp = null!, string[] including = null!);
        void Update(T Item);
        Task DeleteById(int Id);


    }
}
