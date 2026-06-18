using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EduPulse.Core.Dtos.Academics;
using EduPulse.Core.Services.Interfaces;

namespace EduPulse.Api.Controllers
{
    [ApiController]
    [Route("api/classes")]
    [Authorize]
    public class ClassController : ControllerBase
    {
        private readonly IClassService _service;

        public ClassController(IClassService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClassDto>>> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ClassDto>> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound(new { Message = $"Class standard with ID {id} was not found." });
            }
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = "SchoolAdminOnly")]
        public async Task<ActionResult<ClassDto>> Create([FromBody] CreateClassRequest request)
        {
            try
            {
                var result = await _service.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = result.ClassId }, result);
            }
            catch (FluentValidation.ValidationException valEx)
            {
                return BadRequest(new { Message = valEx.Message });
            }
        }

        [HttpPut("{id:guid}")]
        [Authorize(Policy = "SchoolAdminOnly")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateClassRequest request)
        {
            try
            {
                var success = await _service.UpdateAsync(id, request);
                if (!success)
                {
                    return NotFound(new { Message = $"Class standard with ID {id} was not found." });
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
                return NotFound(new { Message = $"Class standard with ID {id} was not found." });
            }
            return NoContent();
        }
    }
}
