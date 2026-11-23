using LibraryManagementAPI.DTOs.Books;
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
    [Tags("Books")]
    public class BooksController(IBookService bookService) : WebApiController
    {
        [HttpGet]
        [ProducesResponseType(typeof(List<ReadBookDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ReadBookDto>>> Get(
            [FromQuery] PaginationParameters pagination,
            [FromQuery] BookFilteringParameters bookFilteringParameters,
            [FromQuery] SortingParameters<BookSortBy> sortingParameters)
        {
            var result = await bookService.GetAsync(pagination, bookFilteringParameters, sortingParameters);
            if (result.IsError)
            {
                return Problem(result.Errors);
            }

            Response.Headers.Append("X-Total-Count", result.Value.TotalCount.ToString());
            Response.Headers.Append("X-Total-Pages", result.Value.TotalPages.ToString());
            return Ok(result.Value.Items);
        }

        [ProducesResponseType<ReadBookDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ReadBookDto>> GetById(int id)
        {
            var result = await bookService.GetByIdAsync(id);
            return result.IsError
                ? Problem(result.Errors)
                : Ok(result.Value);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ReadBookDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ReadBookDto>> Create(CreateBookDto request)
        {
            var result = await bookService.CreateAsync(request);
            return result.IsError
                ? Problem(result.Errors)
                : CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ReadBookDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ReadBookDto>> Update(int id, UpdateBookDto request)
        {
            var result = await bookService.UpdateAsync(id, request);
            return result.IsError
                ? Problem(result.Errors)
                : Ok(result.Value);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await bookService.DeleteAsync(id);
            return result.IsError
                ? Problem(result.Errors)
                : NoContent();
        }
    }
}
