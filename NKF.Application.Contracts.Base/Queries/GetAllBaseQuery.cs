using System.Text.Json.Serialization;

namespace NKF.Application.Contracts.Base.Queries;

public class GetAllBaseQuery
{
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
        get;
        set
        {
            if (value is null)
            {
                SortsQuery = null;
                field = null;
                return;
            }

            List<SortQuery> list = value.Select(x => new SortQuery()
                { SortBy = x.Split("|").First(), SortDirection = x.Split("|").Last() }).ToList();

            SortsQuery = list;
            field = value;
        }
    }

    [JsonIgnore] public IList<SortQuery>? SortsQuery { get; set; }
}
