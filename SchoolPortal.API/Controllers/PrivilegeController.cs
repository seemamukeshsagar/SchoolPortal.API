using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolPortal.API.DTOs.Privilege;
using SchoolPortal.API.Interfaces.Services;

namespace SchoolPortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PrivilegeController : ControllerBase
    {
        private readonly IPrivilegeService _privilegeService;

        public PrivilegeController(IPrivilegeService privilegeService)
        {
            _privilegeService = privilegeService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PrivilegeListDto>>> GetPrivileges()
        {
            var privileges = await _privilegeService.GetAllPrivilegesAsync();
            return Ok(privileges);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PrivilegeDto>> GetPrivilege(Guid id)
        {
            var privilege = await _privilegeService.GetPrivilegeByIdAsync(id);
            if (privilege == null)
            {
                return NotFound();
            }
            return Ok(privilege);
        }

        [HttpPost]
        public async Task<ActionResult<PrivilegeDto>> CreatePrivilege(CreatePrivilegeRequest request)
        {
            var privilege = await _privilegeService.CreatePrivilegeAsync(request);
            return CreatedAtAction(nameof(GetPrivilege), new { id = privilege.Id }, privilege);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePrivilege(Guid id, UpdatePrivilegeRequest request)
        {
            var result = await _privilegeService.UpdatePrivilegeAsync(id, request);
            if (result == null)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrivilege(Guid id)
        {
            var result = await _privilegeService.DeletePrivilegeAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}