using LibraryManagementAPI.DTOs.Books;
using LibraryManagementAPI.Entities;
using Riok.Mapperly.Abstractions;

namespace LibraryManagementAPI.Mapping;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class BookMapper
{
    [MapProperty("Category.Name", nameof(ReadBookDto.CategoryName))]
    public partial ReadBookDto ToReadDto(Book entity);
    [MapperIgnoreTarget(nameof(Book.Id))]
    [MapperIgnoreTarget(nameof(Book.Authors))]
    [MapperIgnoreTarget(nameof(Book.Category))]
    [MapperIgnoreTarget(nameof(Book.AvailableCopies))]
    public partial Book ToEntity(CreateBookDto dto);
    [MapperIgnoreTarget(nameof(Book.Id))]
    [MapperIgnoreTarget(nameof(Book.Authors))]
    [MapperIgnoreTarget(nameof(Book.Category))]
    [MapperIgnoreTarget(nameof(Book.AvailableCopies))]
    public partial void UpdateEntity(UpdateBookDto dto, [MappingTarget] Book entity);
}