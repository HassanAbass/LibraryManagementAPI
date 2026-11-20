using LibraryManagementAPI.RequestParameters.Enums;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementAPI.RequestParameters;

public record SortingParameters<TSortBy> where TSortBy : struct, Enum
{
    [FromQuery(Name = "sortDirection")]
    public SortDirection? SortDirection { get; init; } = Enums.SortDirection.Asc;

    [FromQuery(Name = "sortBy")]
    public TSortBy? SortBy { get; init; } = null;
}
