using FluentValidation;
using LibraryManagementAPI.DataAccess;
using LibraryManagementAPI.DTOs.Books;
using LibraryManagementAPI.Filter;
using LibraryManagementAPI.Mapping;
using LibraryManagementAPI.RequestParameters;
using LibraryManagementAPI.RequestParameters.Enums;
using LibraryManagementAPI.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementAPI.Controllers
{
    [ServiceFilter(typeof(LogActionFilter))]
    [ApiController]
    [Route("api/[controller]")]
    [Tags("Books")]
    public class BooksController(LibraryDbContext libraryDbContext, BookMapper bookMapper, IBookService bookService) : WebApiController
    {
        [HttpGet]
        [ProducesResponseType(typeof(List<ReadBookDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ReadBookDto>>> Get([FromQuery] PaginationParameters pagination, [FromQuery] BookFilteringParameters bookFilteringParameters, [FromQuery] SortingParameters<BookSortBy> sortingParameters)
        {
            var result = await bookService.Get(pagination, bookFilteringParameters, sortingParameters);

            if (result.IsError)
            {
                return Problem(result.FirstError);
            }

            var totalPages = (int)Math.Ceiling((double)result.Value.Count / pagination.PageSize);
            Response.Headers.Append("X-Total-Count", result.Value.Count.ToString());
            Response.Headers.Append("X-Total-Pages", totalPages.ToString());
            return Ok(result.Value);
        }

        [ProducesResponseType<ReadBookDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ReadBookDto>> GetById(int id)
        {
            var book = await libraryDbContext.Books
                .Include(book => book.Category)
                .Include(book => book.Authors)
                .FirstOrDefaultAsync(book => book.Id == id);
            if (book is null)
            {
                return Problem(detail: $"Book with id {id} not found", statusCode: StatusCodes.Status404NotFound);
            }
            return Ok(bookMapper.ToReadDto(book));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ReadBookDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ReadBookDto>> Create(CreateBookDto request)
        {
            var result = await bookService.CreateBookAsync(request);
            if (result.IsError)
            {
                return Problem(result.FirstError);
            }
            return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ReadBookDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ReadBookDto>> Update(int id, UpdateBookDto request)
        {
            var book = await libraryDbContext.Books
                .Include(book => book.Category)
                .Include(book => book.Authors)
                .FirstOrDefaultAsync(book => book.Id == id);
            var authors = await libraryDbContext.Authors
                .Where(author => request.AuthorIds.Contains(author.Id))
                .ToListAsync();

            var category = await libraryDbContext.Categories.FirstOrDefaultAsync(category => category.Id == request.CategoryId);

            if (book is null)
            {
                return Problem(detail: $"Book with id {id} not found", statusCode: StatusCodes.Status404NotFound);
            }

            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return Problem(detail: "Title is required", statusCode: StatusCodes.Status400BadRequest);
            }

            if (await libraryDbContext.Books.AnyAsync(book => book.Id != id && book.Title == request.Title))
            {
                return Problem(detail: "Resource already exists", statusCode: StatusCodes.Status409Conflict);
            }
            if (category is null)
            {
                return Problem(detail: "Selected category not found", statusCode: StatusCodes.Status400BadRequest);
            }
            if (authors.Count != request.AuthorIds.Count)
            {
                return Problem(detail: "One or more selected authors not found", statusCode: StatusCodes.Status400BadRequest);
            }

            bookMapper.UpdateEntity(request, book);
            book.Category = category;
            book.Authors = authors;
            await libraryDbContext.SaveChangesAsync();

            return Ok(bookMapper.ToReadDto(book));
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(int id)
        {
            var book = await libraryDbContext.Books.FirstOrDefaultAsync(book => book.Id == id);
            if (book is null)
            {
                return Problem(detail: $"Book with id {id} not found", statusCode: StatusCodes.Status404NotFound);
            }
            libraryDbContext.Books.Remove(book);
            await libraryDbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}
