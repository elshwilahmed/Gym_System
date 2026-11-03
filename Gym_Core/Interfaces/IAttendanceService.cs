using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gym_Core.DTOs;

namespace Gym_Core.Interfaces
{
    public interface IAttendanceService
    {
        Task<AttendanceResponseDto> CheckInAsync(int id);
        Task CancelCheckInAsync(int id);
        Task<ReportResponseDTO> GetReportAsync(AttendanceReportQueryDto query);
    }
}
