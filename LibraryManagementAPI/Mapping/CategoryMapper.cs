using LibraryManagementAPI.DTOs.Categories;
using LibraryManagementAPI.Entities;
using Riok.Mapperly.Abstractions;

namespace LibraryManagementAPI.Mapping;

[Mapper]
public partial class CategoryMapper
{
    [MapperIgnoreTarget(nameof(Category.Id))]
    public partial Category ToEntity(CategoryCreateDto dto);

    public partial CategoryReadDto ToReadDto(Category entity);
}
