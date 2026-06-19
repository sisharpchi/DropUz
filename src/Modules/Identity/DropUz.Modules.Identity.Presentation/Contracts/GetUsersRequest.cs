using DropUz.Common.Application.Pagination;

namespace DropUz.Modules.Identity.Presentation.Contracts;

public sealed record GetUsersRequest
{
    public int? PageNumber { get; init; }

    public int? PageSize { get; init; }

    public string? Search { get; init; }

    public string? PhoneNumber { get; init; }

    public string? Role { get; init; }

    public string? SortBy { get; init; }

    public string? SortDirection { get; init; }

    public PageRequest ToPageRequest() => new(PageNumber ?? 1, PageSize ?? 20);
}
