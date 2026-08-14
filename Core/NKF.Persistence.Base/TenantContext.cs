namespace NKF.Persistence.Base;

public class TenantContext
{
    public Guid TenantId { get; set; }
    public string? Name { get; set; }
    public string? Slug { get; set; }
    public string? Domain { get; set; }

    public string? ConnectionString { get; set; }
}