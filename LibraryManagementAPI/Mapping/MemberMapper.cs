using LibraryManagementAPI.DTOs.Members;
using LibraryManagementAPI.Entities;
using Riok.Mapperly.Abstractions;

namespace LibraryManagementAPI.Mapping
{
    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class MemberMapper
    {
        public partial ReadMemberDto ToReadDto(Members entity);

        [MapperIgnoreTarget(nameof(Members.Id))]
        [MapperIgnoreTarget(nameof(Members.IsActive))]
        public partial Members ToEntity(CreateMemberDto dto);

        [MapperIgnoreTarget(nameof(Members.Id))]
        public partial void UpdateEntity(UpdateMemberDto dto, [MappingTarget] Members entity);

    }
}
