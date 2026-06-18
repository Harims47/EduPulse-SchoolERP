using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EduPulse.Core.Dtos.Staff;
using EduPulse.Core.Services.Interfaces;

namespace EduPulse.Api.Controllers
{
    [ApiController]
    [Route("api/staff")]
    [Authorize]
    public class StaffController : ControllerBase
    {
        private readonly IStaffService _service;

        public StaffController(IStaffService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StaffDto>>> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<StaffDto>> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound(new { Message = $"Staff member with ID {id} was not found." });
            }
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = "SchoolAdminOnly")]
        public async Task<ActionResult<StaffDto>> Create([FromBody] CreateStaffRequest request)
        {
            try
            {
                var result = await _service.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = result.StaffId }, result);
            }
            catch (FluentValidation.ValidationException valEx)
            {
                return BadRequest(new { Message = valEx.Message });
            }
        }

        [HttpPut("{id:guid}")]
        [Authorize(Policy = "SchoolAdminOnly")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateStaffRequest request)
        {
            try
            {
                var success = await _service.UpdateAsync(id, request);
                if (!success)
                {
                    return NotFound(new { Message = $"Staff member with ID {id} was not found." });
                }
                return NoContent();
            }
            catch (FluentValidation.ValidationException valEx)
            {
                return BadRequest(new { Message = valEx.Message });
            }
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Policy = "SchoolAdminOnly")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _service.DeleteAsync(id);
            if (!success)
            {
                return NotFound(new { Message = $"Staff member with ID {id} was not found." });
            }
            return NoContent();
        }
    }
}
