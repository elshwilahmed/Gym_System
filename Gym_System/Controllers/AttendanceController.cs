using Gym_Core.DTOs;
using Gym_Core.Interfaces;
using Gym_Core.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gym_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Receptionist")]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService attendanceService;

        public AttendanceController(IAttendanceService attendanceService)
        {
            this.attendanceService = attendanceService;
        }

        [HttpPost]
        [Route("CheckIn/{memberId:int}")]
        public async Task<ActionResult> CheckIn(int memberId)
        {
            return Ok(APIResponse<AttendanceResponseDto>
                .Success(await attendanceService.CheckInAsync(memberId),
                         "Check-in recorded successfully"));
        }

        [HttpDelete]
        [Route("CancelCheckIn/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> CancelCheckIn(int id)
        {
            await attendanceService.CancelCheckInAsync(id);

            return Ok(APIResponse<string>
                .Success(null!, "Check-in cancelled successfully"));
        }

        [HttpGet]
        [Route("GetReport")]
        public async Task<ActionResult> GetReport([FromQuery] AttendanceReportQueryDto query)
        {
            return Ok(APIResponse<ReportResponseDTO>
                .Success(await attendanceService.GetReportAsync(query)));
        }
    }
}