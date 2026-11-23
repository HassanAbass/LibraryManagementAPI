using LibraryManagementAPI.DTOs.Authors;
using LibraryManagementAPI.Filter;
using LibraryManagementAPI.RequestParameters;
using LibraryManagementAPI.RequestParameters.Enums;
using LibraryManagementAPI.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementAPI.Controllers
{
    [ServiceFilter(typeof(LogActionFilter))]
    [ApiController]
    [Route("api/[controller]")]
    [Tags("Authors")]
    public class AuthorsController(IAuthorService authorService) : WebApiController
    {
        [HttpGet]
        [ProducesResponseType(typeof(List<ReadAuthorDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ReadAuthorDto>>> Get([FromQuery] PaginationParameters pagination)
        {
            var result = await authorService.GetAllAsync(pagination);
            HttpContext.Response.Headers.Append("X-Total-Count", result.Value.Count.ToString());
            HttpContext.Response.Headers.Append("X-Total-Pages", Math.Ceiling((double)result.Value.Count / pagination.PageSize).ToString() );
            return result.IsError ? Problem(result.Errors) : Ok(result.Value);
        }

        [ProducesResponseType<ReadAuthorDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ReadAuthorDto>> GetById(int id)
        {
            var result = await authorService.GetByIdAsync(id);
            return result.IsError ? Problem(result.Errors) : Ok(result.Value);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ReadAuthorDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ReadAuthorDto>> Create(CreateAuthorDto request)
        {
            var result = await authorService.CreateAsync(request);
            return result.IsError
                ? Problem(result.Errors)
                : CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ReadAuthorDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ReadAuthorDto>> Update(int id, UpdateAuthorDto request)
        {
            var result = await authorService.UpdateAsync(id, request);
            return result.IsError ? Problem(result.Errors) : Ok(result.Value);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await authorService.DeleteAsync(id);
            return result.IsError ? Problem(result.Errors) : NoContent();
        }
    }
}
