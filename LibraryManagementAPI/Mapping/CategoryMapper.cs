using LibraryManagementAPI.DTOs.Categories;
using LibraryManagementAPI.Entities;
using Riok.Mapperly.Abstractions;

namespace LibraryManagementAPI.Mapping;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CategoryMapper
{
    [MapperIgnoreTarget(nameof(Category.Id))]
    [MapperIgnoreTarget(nameof(Category.Books))]
    public partial Category ToEntity(CreateCategoryDto dto);

    public partial ReadCategoryDto ToReadDto(Category entity);
}
