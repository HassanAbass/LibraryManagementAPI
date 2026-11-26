using LibraryManagementAPI.DTOs.Members;
using LibraryManagementAPI.Filter;
using LibraryManagementAPI.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementAPI.Controllers
{
    [ServiceFilter(typeof(LogActionFilter))]
    [ApiController]
    [Route("api/[controller]")]
    [Tags("Members")]
    public class MembersController(IMemberService memberService) : WebApiController
    {
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ReadMemberDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ReadMemberDto>>> Get()
        {
            var result = await memberService.GetAllMembersAsync();
            return result.IsError ? Problem(result.Errors) : Ok(result.Value);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ReadMemberDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ReadMemberDto>> GetById(int id)
        {
            var result = await memberService.GetByIdAsync(id);
            return result.IsError ? Problem(result.Errors) : Ok(result.Value);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ReadMemberDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ReadMemberDto>> Create(CreateMemberDto request)
        {
            var result = await memberService.CreateAsync(request);
            return result.IsError
                ? Problem(result.Errors)
                : CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ReadMemberDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ReadMemberDto>> Update(int id, UpdateMemberDto request)
        {
            var result = await memberService.UpdateAsync(id, request);
            return result.IsError ? Problem(result.Errors) : Ok(result.Value);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await memberService.DeleteAsync(id);
            return result.IsError ? Problem(result.Errors) : NoContent();
        }
    }
}
