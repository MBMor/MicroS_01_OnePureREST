using InventoryService.Application.Common.Models;
using InventoryService.Application.Products.Models;

namespace InventoryService.Application.Products.Requests;

public sealed class ProductListRequest
{
    public const int DefaultPage = 1;
    public const int DefaultPageSize = 20;
    public const int MaxPageSize = 100;

    public int Page { get; init; } = DefaultPage;

    public int PageSize { get; init; } = DefaultPageSize;

    public string? Name { get; init; }

    public string? Sku { get; init; }

    public bool? IsActive { get; init; }

    public ProductSortBy SortBy { get; init; } = ProductSortBy.CreatedAt;

    public SortDirection SortDirection { get; init; } = SortDirection.Desc;
}