namespace LibraryManagementAPI.DTOs.Members;

public sealed record ReadMemberDto(int Id, string FirstName, string LastName, bool IsActive);
