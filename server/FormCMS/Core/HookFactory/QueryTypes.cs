using System.Collections.Immutable;
using FormCMS.Core.Descriptors;

namespace FormCMS.Core.HookFactory;

public record QueryPreGetListArgs(
    string Name,
    string EntityName,
    ImmutableArray<ValidFilter> Filters,
    ImmutableArray<ValidSort> Sorts,
    ValidSpan Span,
    ValidPagination Pagination,
    Record[]? OutRecords = default) : BaseArgs(Name);

public record QueryPreGetManyArgs(
    string Name,
    string EntityName,
    ImmutableArray<ValidFilter> Filters,
    ValidPagination Pagination,
    Record[]? OutRecords = default) : BaseArgs(Name);
public record QueryPreGetSingleArgs(string Name,string EntityName, ImmutableArray<ValidFilter> Filters, Record? OutRecord = default):BaseArgs(Name) ;