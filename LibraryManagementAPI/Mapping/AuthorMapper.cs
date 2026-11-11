using LibraryManagementAPI.DTOs.Authors;
using LibraryManagementAPI.Entities;
using Riok.Mapperly.Abstractions;

namespace LibraryManagementAPI.Mapping;

[Mapper]
public partial class AuthorMapper
{
    [MapperIgnoreTarget(nameof(Author.Id))]
    public partial Author ToEntity(AuthorCreateDto dto);

    public partial AuthorReadDto ToReadDto(Author entity);
    [MapperIgnoreTarget(nameof(Author.Id))]
    public partial void UpdateEntity(AuthorUpdateDto dto, [MappingTarget] Author entity);
}
