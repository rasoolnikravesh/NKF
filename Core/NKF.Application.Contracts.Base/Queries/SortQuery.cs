namespace NKF.Application.Contracts.Base.Queries;

public class SortQuery
{
    public string SortBy { get; set; } = "RegisterDate";
    public string SortDirection { get; set; } = "desc";
}