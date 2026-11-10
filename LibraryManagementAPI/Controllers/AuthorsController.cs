using LibraryManagementAPI.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorsController : ControllerBase
    {
        private static List<Author> _authors = new([
            new Author { Id = 1, FirstName = "Jane", LastName = "Austen", YearOfBirth = 1775 },
            new Author { Id = 2, FirstName = "Mark", LastName = "Twain", YearOfBirth = 1835 },
            new Author { Id = 3, FirstName = "Virginia", LastName = "Woolf", YearOfBirth = 1882 },
            new Author { Id = 4, FirstName = "George", LastName = "Orwell", YearOfBirth = 1903 },
            new Author { Id = 5, FirstName = "Toni", LastName = "Morrison", YearOfBirth = 1931 },
            new Author { Id = 6, FirstName = "Gabriel", LastName = "Marquez", YearOfBirth = 1927 },
            new Author { Id = 7, FirstName = "Mary", LastName = "Shelley", YearOfBirth = 1797 },
            new Author { Id = 8, FirstName = "Leo", LastName = "Tolstoy", YearOfBirth = 1828 },
            new Author { Id = 9, FirstName = "Agatha", LastName = "Christie", YearOfBirth = 1890 },
            new Author { Id = 10, FirstName = "James", LastName = "Baldwin", YearOfBirth = 1924 },
        ]);

        [HttpGet]
        [ProducesResponseType(typeof(List<Author>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<Author>>> Get()
        {
            return Ok(_authors);
        }

        [ProducesResponseType<Author>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Author>> GetById(int id)
        {
            var author = _authors.FirstOrDefault(a => a.Id == id);
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
            if (_authors.Any(a => a.Id == author.Id) || _authors.Any(a => a.FirstName == author.FirstName && a.LastName == author.LastName))
            {
                return Problem(detail: "Resource already exists", statusCode: StatusCodes.Status409Conflict);
            }
            if (string.IsNullOrWhiteSpace(author.FirstName) || string.IsNullOrWhiteSpace(author.LastName))
            {
                return Problem(detail: "First name and last name are required", statusCode: StatusCodes.Status400BadRequest);
            }

            if (_authors.Find(a => a.Id == author.Id) is not null)
            {
                return Problem(detail: $"Author with id {author.Id} already exists", statusCode: StatusCodes.Status409Conflict);
            }
            _authors.Add(author);

            return CreatedAtAction(nameof(GetById), new { id = author.Id }, author);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(Author), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<Author>> Update(int id, Author request)
        {
            var author = _authors.FirstOrDefault(a => a.Id == id);
            if (author is null)
            {
                return Problem(detail: $"Author with id {id} not found", statusCode: StatusCodes.Status404NotFound);
            }

            if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName))
            {
                return Problem(detail: "First name and last name are required", statusCode: StatusCodes.Status400BadRequest);
            }

            if (_authors.Any(a => a.Id != id && a.FirstName == request.FirstName && a.LastName == request.LastName))
            {
                return Problem(detail: "Resource already exists", statusCode: StatusCodes.Status409Conflict);
            }

            author.FirstName = request.FirstName;
            author.LastName = request.LastName;
            author.YearOfBirth = request.YearOfBirth;

            return Ok(author);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(int id)
        {
            var author = _authors.FirstOrDefault(author => author.Id == id);
            if (author is null)
            {
                return Problem(detail: $"Author with id {id} not found", statusCode: StatusCodes.Status404NotFound);
            }
            _authors.Remove(author);
            return NoContent();
        }
    }
}
