using System.Text.Json.Serialization;

namespace LibraryManagementAPI.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum LoanStatus
{
    Active,
    Overdue,
    Returned
}
