using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EduPulse.Core.Dtos.Students;
using EduPulse.Core.Services.Interfaces;

namespace EduPulse.Api.Controllers
{
    [ApiController]
    [Route("api/students/{studentId:guid}/guardians")]
    [Authorize]
    public class StudentGuardianController : ControllerBase
    {
        private readonly IStudentGuardianService _service;

        public StudentGuardianController(IStudentGuardianService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentGuardianDto>>> GetGuardians(Guid studentId)
        {
            try
            {
                var result = await _service.GetGuardiansByStudentIdAsync(studentId);
                return Ok(result);
            }
            catch (FluentValidation.ValidationException valEx)
            {
                return BadRequest(new { Message = valEx.Message });
            }
        }

        [HttpPost]
        [Authorize(Policy = "SchoolAdminOnly")]
        public async Task<IActionResult> LinkGuardian(Guid studentId, [FromBody] LinkGuardianRequest request)
        {
            try
            {
                var success = await _service.LinkGuardianAsync(studentId, request);
                if (!success)
                {
                    return BadRequest(new { Message = "Could not link guardian to student." });
                }
                return NoContent();
            }
            catch (FluentValidation.ValidationException valEx)
            {
                return BadRequest(new { Message = valEx.Message });
            }
        }

        [HttpPut("{guardianId:guid}")]
        [Authorize(Policy = "SchoolAdminOnly")]
        public async Task<IActionResult> UpdateRelationship(Guid studentId, Guid guardianId, [FromBody] UpdateRelationshipRequest request)
        {
            try
            {
                var success = await _service.UpdateRelationshipAsync(studentId, guardianId, request);
                if (!success)
                {
                    return NotFound(new { Message = "Student-Guardian relationship mapping was not found." });
                }
                return NoContent();
            }
            catch (FluentValidation.ValidationException valEx)
            {
                return BadRequest(new { Message = valEx.Message });
            }
        }

        [HttpDelete("{guardianId:guid}")]
        [Authorize(Policy = "SchoolAdminOnly")]
        public async Task<IActionResult> UnlinkGuardian(Guid studentId, Guid guardianId)
        {
            var success = await _service.UnlinkGuardianAsync(studentId, guardianId);
            if (!success)
            {
                return NotFound(new { Message = "Student-Guardian relationship mapping was not found." });
            }
            return NoContent();
        }
    }
}
