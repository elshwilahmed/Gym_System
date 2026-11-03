using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym_Core.DTOs
{
    public class AttendanceReportQueryDto
    {
 
        public DateTime? From { get; set; }

        public DateTime? To { get; set; }

        public int? MemberId { get; set; }
    }
}
