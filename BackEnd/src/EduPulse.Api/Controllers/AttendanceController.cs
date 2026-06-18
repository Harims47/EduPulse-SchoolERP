using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EduPulse.Core.Dtos.Attendance;
using EduPulse.Core.Services.Interfaces;

namespace EduPulse.Api.Controllers
{
    [ApiController]
    [Route("api/attendance")]
    [Authorize]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _service;

        public AttendanceController(IAttendanceService service)
        {
            _service = service;
        }

        [HttpPost("mark")]
        [Authorize(Policy = "AdminOrAccountant")]
        public async Task<IActionResult> Mark([FromBody] MarkAttendanceRequest request)
        {
            try
            {
                var success = await _service.MarkAttendanceAsync(request);
                if (!success)
                {
                    return BadRequest(new { Message = "Could not register attendance details." });
                }
                return NoContent();
            }
            catch (FluentValidation.ValidationException valEx)
            {
                return BadRequest(new { Message = valEx.Message });
            }
        }

        [HttpGet("daily")]
        public async Task<ActionResult<DailyAttendanceReport>> GetDaily([FromQuery] Guid classId, [FromQuery] Guid sectionId, [FromQuery] DateTime date)
        {
            var result = await _service.GetDailyAttendanceAsync(classId, sectionId, date);
            if (result == null)
            {
                return NotFound(new { Message = "Attendance session for the selected class/section and date was not found." });
            }
            return Ok(result);
        }

        [HttpGet("student/{studentId:guid}")]
        public async Task<ActionResult<IEnumerable<StudentAttendanceHistoryDto>>> GetStudentHistory(Guid studentId)
        {
            try
            {
                var result = await _service.GetStudentHistoryAsync(studentId);
                return Ok(result);
            }
            catch (FluentValidation.ValidationException valEx)
            {
                return BadRequest(new { Message = valEx.Message });
            }
        }

        [HttpGet("class/{classId:guid}")]
        public async Task<ActionResult<IEnumerable<ClassAttendanceSummaryDto>>> GetClassSummary(
            Guid classId, 
            [FromQuery] DateTime? startDate = null, 
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var result = await _service.GetClassSummaryAsync(classId, startDate, endDate);
                return Ok(result);
            }
            catch (FluentValidation.ValidationException valEx)
            {
                return BadRequest(new { Message = valEx.Message });
            }
        }
    }
}
