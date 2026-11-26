using LibraryManagementAPI.DTOs.Loans;
using LibraryManagementAPI.Entities;
using Riok.Mapperly.Abstractions;

namespace LibraryManagementAPI.Mapping;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class LoanMapper
{
    [MapperIgnoreTarget(nameof(Loan.Id))]
    [MapperIgnoreTarget(nameof(Loan.Member))]
    [MapperIgnoreTarget(nameof(Loan.Book))]
    [MapperIgnoreTarget(nameof(Loan.LoanDate))]
    [MapperIgnoreTarget(nameof(Loan.ReturnedAt))]
    public partial Loan ToEntity(CreateLoanDto dto);

    public partial ReadLoanDto ToReadDto(Loan entity);

    [MapperIgnoreTarget(nameof(Loan.Id))]
    [MapperIgnoreTarget(nameof(Loan.MemberId))]
    [MapperIgnoreTarget(nameof(Loan.Member))]
    [MapperIgnoreTarget(nameof(Loan.BookId))]
    [MapperIgnoreTarget(nameof(Loan.Book))]
    [MapperIgnoreTarget(nameof(Loan.LoanDate))]
    [MapperIgnoreTarget(nameof(Loan.ReturnedAt))]
    public partial void UpdateEntity(UpdateLoanDto dto, [MappingTarget] Loan entity);
}
