using LibraryManagementAPI.DataAccess;
using LibraryManagementAPI.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Tags("Authors")]
    public class AuthorsController(LibraryDbContext libraryDbContext) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(List<Author>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<Author>>> Get()
        {
            return Ok(await libraryDbContext.Authors.ToListAsync());
        }

        [ProducesResponseType<Author>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Author>> GetById(int id)
        {
            var author = await libraryDbContext.Authors.FirstOrDefaultAsync(a => a.Id == id);
            if (author is null)
            {
                return Problem(detail: $"Author with id {id} not found", statusCode: StatusCodes.Status404NotFound);
            }
            return Ok(author);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Author), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<Author>> Create(Author author)
        {
            if (await libraryDbContext.Authors.AnyAsync(a => a.Id == author.Id) || await libraryDbContext.Authors.AnyAsync(a => a.FirstName == author.FirstName && a.LastName == author.LastName))
            {
                return Problem(detail: "Resource already exists", statusCode: StatusCodes.Status409Conflict);
            }
            if (string.IsNullOrWhiteSpace(author.FirstName) || string.IsNullOrWhiteSpace(author.LastName))
            {
                return Problem(detail: "First name and last name are required", statusCode: StatusCodes.Status400BadRequest);
            }

            if (await libraryDbContext.Authors.FirstOrDefaultAsync(a => a.Id == author.Id) is not null)
            {
                return Problem(detail: $"Author with id {author.Id} already exists", statusCode: StatusCodes.Status409Conflict);
            }
            libraryDbContext.Authors.Add(author);
            await libraryDbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = author.Id }, author);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(Author), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<Author>> Update(int id, Author request)
        {
            var author = await libraryDbContext.Authors.FirstOrDefaultAsync(a => a.Id == id);
            if (author is null)
            {
                return Problem(detail: $"Author with id {id} not found", statusCode: StatusCodes.Status404NotFound);
            }

            if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName))
            {
                return Problem(detail: "First name and last name are required", statusCode: StatusCodes.Status400BadRequest);
            }

            if (await libraryDbContext.Authors.AnyAsync(a => a.Id != id && a.FirstName == request.FirstName && a.LastName == request.LastName))
            {
                return Problem(detail: "Resource already exists", statusCode: StatusCodes.Status409Conflict);
            }

            author.FirstName = request.FirstName;
            author.LastName = request.LastName;
            author.YearOfBirth = request.YearOfBirth;
            await libraryDbContext.SaveChangesAsync();

            return Ok(author);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(int id)
        {
            var author = await libraryDbContext.Authors.FirstOrDefaultAsync(author => author.Id == id);
            if (author is null)
            {
                return Problem(detail: $"Author with id {id} not found", statusCode: StatusCodes.Status404NotFound);
            }
            libraryDbContext.Authors.Remove(author);
            await libraryDbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}
