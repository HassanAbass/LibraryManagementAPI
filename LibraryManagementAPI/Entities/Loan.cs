using System.ComponentModel.DataAnnotations.Schema;
using LibraryManagementAPI.Enums;

namespace LibraryManagementAPI.Entities;

public class Loan
{
    public int Id { get; set; }
    public int MemberId { get; set; }
    public Members Member { get; set; } = null!;
    public int BookId { get; set; }
    public Book Book { get; set; } = null!;
    public DateTime LoanDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnedAt { get; set; }

    [NotMapped]
    public LoanStatus Status => ReturnedAt.HasValue
        ? LoanStatus.Returned
        : DueDate < DateTime.UtcNow
            ? LoanStatus.Overdue
            : LoanStatus.Active;
}
