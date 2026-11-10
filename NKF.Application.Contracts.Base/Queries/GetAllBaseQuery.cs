using System.Text.Json.Serialization;

namespace NKF.Application.Contracts.Base.Queries;

public class GetAllBaseQuery
{
    private IList<string>? _sorts;

    public GetAllBaseQuery()
    {
        if (Sorts is not null && Sorts.Any())
        {
            SortsQuery = Sorts.Select(x => new SortQuery()
            {
                SortBy = x.Split("|").First(),
                SortDirection = x.Split("|").Last()
            }).ToList();
        }
    }

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public IList<string>? Sorts
    {
        get => _sorts;
        set
        {
            if (value is null)
            {
                SortsQuery = null;
                _sorts = null;
                return;
            }

            SortsQuery = value.Select(x => new SortQuery()
            {
                SortBy = x.Split("|").First(),
                SortDirection = x.Split("|").Last()
            }).ToList();
            _sorts = value;
        }
    }

    [JsonIgnore] public IList<SortQuery>? SortsQuery { get; set; }
}
