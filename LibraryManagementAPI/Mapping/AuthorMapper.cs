using LibraryManagementAPI.DTOs.Authors;
using LibraryManagementAPI.Entities;
using Riok.Mapperly.Abstractions;

namespace LibraryManagementAPI.Mapping;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class AuthorMapper
{
    [MapperIgnoreTarget(nameof(Author.Id))]
    [MapperIgnoreTarget(nameof(Author.Books))]
    public partial Author ToEntity(CreateAuthorDto dto);

    public partial ReadAuthorDto ToReadDto(Author entity);
    [MapperIgnoreTarget(nameof(Author.Id))]
    [MapperIgnoreTarget(nameof(Author.Books))]
    public partial void UpdateEntity(UpdateAuthorDto dto, [MappingTarget] Author entity);
}
