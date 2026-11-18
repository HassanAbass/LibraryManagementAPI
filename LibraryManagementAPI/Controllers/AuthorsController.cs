using FluentValidation;
using LibraryManagementAPI.DataAccess;
using LibraryManagementAPI.DTOs.Authors;
using LibraryManagementAPI.Filter;
using LibraryManagementAPI.Mapping;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementAPI.Controllers
{
    [ServiceFilter(typeof(LogActionFilter))]
    [ApiController]
    [Route("api/[controller]")]
    [Tags("Authors")]
    public class AuthorsController(LibraryDbContext libraryDbContext, AuthorMapper authorMapper, IValidator<CreateAuthorDto> createAuthorValidator) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(List<ReadAuthorDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ReadAuthorDto>>> Get()
        {
            var authors = await libraryDbContext.Authors.Include(author => author.Books).ToListAsync();
            return Ok(authors.Select(authorMapper.ToReadDto).ToList());
        }

        [ProducesResponseType<ReadAuthorDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ReadAuthorDto>> GetById(int id)
        {
            var author = await libraryDbContext.Authors.FirstOrDefaultAsync(author => author.Id == id);
            if (author is null)
            {
                return Problem(detail: $"Author with id {id} not found", statusCode: StatusCodes.Status404NotFound);
            }
            return Ok(authorMapper.ToReadDto(author));
        }
 
        [HttpPost]
        [ProducesResponseType(typeof(ReadAuthorDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ReadAuthorDto>> Create(CreateAuthorDto request)
        {
            var validationResult = await createAuthorValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return ValidationProblem(new ValidationProblemDetails(validationResult.ToDictionary()));
            }

            if (await libraryDbContext.Authors.AnyAsync(author => author.FirstName == request.FirstName && author.LastName == request.LastName))
            {
                return Problem(detail: "Resource already exists", statusCode: StatusCodes.Status409Conflict);
            }

            var author = authorMapper.ToEntity(request);
            libraryDbContext.Authors.Add(author);
            await libraryDbContext.SaveChangesAsync();

            var response = authorMapper.ToReadDto(author);
            return CreatedAtAction(nameof(GetById), new { id = author.Id }, response);
        }
            
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ReadAuthorDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ReadAuthorDto>> Update(int id, UpdateAuthorDto request)
        {
            var author = await libraryDbContext.Authors.FirstOrDefaultAsync(author => author.Id == id);
            if (author is null)
            {
                return Problem(detail: $"Author with id {id} not found", statusCode: StatusCodes.Status404NotFound);
            }

            if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName))
            {
                return Problem(detail: "First name and last name are required", statusCode: StatusCodes.Status400BadRequest);
            }

            if (await libraryDbContext.Authors.AnyAsync(author => author.Id != id && author.FirstName == request.FirstName && author.LastName == request.LastName))
            {
                return Problem(detail: "Resource already exists", statusCode: StatusCodes.Status409Conflict);
            }

            authorMapper.UpdateEntity(request, author);
            await libraryDbContext.SaveChangesAsync();

            return Ok(authorMapper.ToReadDto(author));
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
