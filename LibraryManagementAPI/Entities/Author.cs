namespace LibraryManagementAPI.Entities;

public class Author
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public int? YearOfBirth { get; set; }
    public int Id { get; set; }
}
