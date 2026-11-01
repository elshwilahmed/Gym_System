using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Gym_Core.Models
{
    public class AppUser : IdentityUser<int>
    {
        /*
   {
        "firstName": "Super",
        "lastName": "Admin",
        "userName": "super_admin",
        "email": "admin@gym.com",
        "password": "Password@2026",
        "role": "Admin"
   } 
*/

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = String.Empty;
        [MaxLength(11), MinLength(11)]
        public string Phone { get; set; } = String.Empty;

        public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();

    }
}
