using ErrorOr;
using FluentValidation;
using LibraryManagementAPI.DataAccess;
using LibraryManagementAPI.DTOs.Authors;
using LibraryManagementAPI.Errors;
using LibraryManagementAPI.Mapping;
using LibraryManagementAPI.RequestParameters;
using LibraryManagementAPI.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementAPI.Services;

public sealed class AuthorService(
    LibraryDbContext dbContext,
    AuthorMapper authorMapper,
    IValidator<CreateAuthorDto> createAuthorValidator) : IAuthorService
{
    public async Task<ErrorOr<List<ReadAuthorDto>>> GetAllAsync(PaginationParameters pagination)
    {
        var authors = await dbContext.Authors
            .Include(author => author.Books)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync();
        return authors.Select(authorMapper.ToReadDto).ToList();
    }

    public async Task<ErrorOr<ReadAuthorDto>> GetByIdAsync(int id)
    {
        var author = await dbContext.Authors.FirstOrDefaultAsync(author => author.Id == id);
        if (author is null)
        {
            return AuthorErrors.NotFound(id);
        }

        return authorMapper.ToReadDto(author);
    }

    public async Task<ErrorOr<ReadAuthorDto>> CreateAsync(CreateAuthorDto request)
    {
        var validationResult = await createAuthorValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors
                .Select(error => Error.Validation(code: error.PropertyName, description: error.ErrorMessage))
                .ToList();
        }

        if (await dbContext.Authors.AnyAsync(author =>
                author.FirstName == request.FirstName && author.LastName == request.LastName))
        {
            return AuthorErrors.Duplicate;
        }

        var author = authorMapper.ToEntity(request);
        dbContext.Authors.Add(author);
        await dbContext.SaveChangesAsync();
        return authorMapper.ToReadDto(author);
    }

    public async Task<ErrorOr<ReadAuthorDto>> UpdateAsync(int id, UpdateAuthorDto request)
    {
        var author = await dbContext.Authors.FirstOrDefaultAsync(author => author.Id == id);
        if (author is null)
        {
            return AuthorErrors.NotFound(id);
        }

        if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName))
        {
            return AuthorErrors.NamesRequired;
        }

        if (await dbContext.Authors.AnyAsync(existing =>
                existing.Id != id && existing.FirstName == request.FirstName && existing.LastName == request.LastName))
        {
            return AuthorErrors.Duplicate;
        }

        authorMapper.UpdateEntity(request, author);
        await dbContext.SaveChangesAsync();
        return authorMapper.ToReadDto(author);
    }

    public async Task<ErrorOr<Deleted>> DeleteAsync(int id)
    {
        var author = await dbContext.Authors.FirstOrDefaultAsync(author => author.Id == id);
        if (author is null)
        {
            return AuthorErrors.NotFound(id);
        }

        dbContext.Authors.Remove(author);
        await dbContext.SaveChangesAsync();
        return Result.Deleted;
    }
}
