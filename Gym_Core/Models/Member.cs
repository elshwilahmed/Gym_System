using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym_Core.Models
{
    public class Member
    {
        public int Id { get; set; }

        [Required, MaxLength(15)]
        public string FirstName { get; set; } = string.Empty;

        [MaxLength(15)]
        public string? MiddleName { get; set; } = string.Empty;

        [Required, MaxLength(15)]
        public string LastName { get; set; } = String.Empty;

        public string FullName { get; private set; } = string.Empty;

        [Required]
        public string Phone { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }
        [NotMapped]
        public int Age
        {
            get
            {
                if (DateOfBirth == default) return 0;

                var today = DateTime.Today;
                var age = today.Year - DateOfBirth.Year; 

                if (DateOfBirth.Date > today.AddYears(-age))
                    age--;

                return age;
            }
        }
        public bool IsActive { get; set; } = true;
        public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    }
}
