using System.Collections.Concurrent;
using System.Net.Http.Json;
using JasperFx.Descriptors;
using JasperFx.MultiTenancy;
using Microsoft.Extensions.Configuration;

namespace NKF.Persistence.Postgres.Base.Services;

public sealed class TenantConnectionStringSource : ITenantedSource<string>
{
    private static readonly HttpClient Client = new();
    private readonly ConcurrentDictionary<string, string> _connectionStrings = new(StringComparer.OrdinalIgnoreCase);
    private readonly string _tenantApiBaseUrl;
    private readonly string _contextName;

    public TenantConnectionStringSource(IConfiguration configuration, string contextName)
    {
        _tenantApiBaseUrl = configuration["Tenant:BaseUrl"]
                            ?? configuration["Tenant__BaseUrl"]
                            ?? "https://localhost:7031";
        _contextName = contextName;
    }

    public DatabaseCardinality Cardinality => DatabaseCardinality.DynamicMultiple;

    public async ValueTask<string> FindAsync(string tenantId)
    {
        if (_connectionStrings.TryGetValue(tenantId, out string? connectionString))
        {
            return connectionString;
        }

        if (Guid.TryParse(tenantId, out _))
        {
            await RefreshAsync();
            if (_connectionStrings.TryGetValue(tenantId, out connectionString))
            {
                return connectionString;
            }

            throw new InvalidOperationException(
                $"Tenant '{tenantId}' has no connection for context '{_contextName}'.");
        }

        string requestUri = $"{_tenantApiBaseUrl.TrimEnd('/')}/Tenant?Slug={Uri.EscapeDataString(tenantId)}&ContextName={Uri.EscapeDataString(_contextName)}";
        TenantApiResponse? response = await Client.GetFromJsonAsync<TenantApiResponse>(requestUri);
        if (response is not { IsSucces: true, Value: not null })
        {
            throw new InvalidOperationException(
                $"Tenant '{tenantId}' has no connection for context '{_contextName}'.");
        }

        _connectionStrings[response.Value.Id.ToString()] = response.Value.ConnectionString;
        _connectionStrings[tenantId] = response.Value.ConnectionString;
        return response.Value.ConnectionString;
    }

    public async Task RefreshAsync()
    {
        string requestUri = $"{_tenantApiBaseUrl.TrimEnd('/')}/Business/get-all-businesses-filter-by-context-name?ContextName={Uri.EscapeDataString(_contextName)}";
        TenantListApiResponse? response;
        try
        {
            response = await Client.GetFromJsonAsync<TenantListApiResponse>(requestUri);
        }
        catch (HttpRequestException)
        {
            return;
        }
        catch (TaskCanceledException)
        {
            return;
        }

        if (response is not { IsSucces: true, Value: not null })
        {
            return;
        }

        foreach (TenantConnection tenant in response.Value)
        {
            _connectionStrings[tenant.Id.ToString()] = tenant.ConnectionString;
        }
    }

    public IReadOnlyList<string> AllActive() => _connectionStrings.Values.Distinct().ToArray();

    public IReadOnlyList<Assignment<string>> AllActiveByTenant() => _connectionStrings
        .Select(tenant => new Assignment<string>(tenant.Key, tenant.Value))
        .ToArray();

    private sealed record TenantApiResponse(bool IsSucces, TenantConnection? Value);
    private sealed record TenantListApiResponse(bool IsSucces, List<TenantConnection>? Value);
    private sealed record TenantConnection(Guid Id, string ConnectionString, string Name);
}
