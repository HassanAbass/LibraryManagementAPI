using FluentValidation;
using LibraryManagementAPI.DataAccess;
using LibraryManagementAPI.DTOs.Books;
using LibraryManagementAPI.Filter;
using LibraryManagementAPI.Mapping;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementAPI.Controllers
{
    [ServiceFilter(typeof(LogActionFilter))]
    [ApiController]
    [Route("api/[controller]")]
    [Tags("Books")]
    public class BooksController(LibraryDbContext libraryDbContext, BookMapper bookMapper) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(List<ReadBookDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ReadBookDto>>> Get()
        {
            var books = await libraryDbContext.Books
                .Include(book => book.Category)
                .Include(book => book.Authors)
                .ToListAsync();
            return Ok(books.Select(bookMapper.ToReadDto).ToList());
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
            var category = await libraryDbContext.Categories
                .FirstOrDefaultAsync(category => category.Id == request.CategoryId);
            if (category is null)
            {
                return Problem(detail: "Category not found", statusCode: StatusCodes.Status400BadRequest);
            }
            var authors = await libraryDbContext.Authors
                .Where(author => request.AuthorIds.Contains(author.Id))
                .ToListAsync();
            if (authors.Count != request.AuthorIds.Count)
            {
                return Problem(detail: "One or more selected authors not found", statusCode: StatusCodes.Status400BadRequest);
            }
            var book = bookMapper.ToEntity(request);
            book.Category = category;
            book.Authors = authors;
            book.AvailableCopies = request.TotalCopies;
            libraryDbContext.Books.Add(book);
            await libraryDbContext.SaveChangesAsync();

            var response = bookMapper.ToReadDto(book);
            return CreatedAtAction(nameof(GetById), new { id = book.Id }, response);
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
