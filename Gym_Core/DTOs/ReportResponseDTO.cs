using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym_Core.DTOs
{
    public class ReportResponseDTO
    {
        public int TotalVisits { get; set; }
        public int UniqueMembers { get; set; }
        public List<AttendanceResponseDto> MembersAttendances { get; set; } = new();
    }
}
