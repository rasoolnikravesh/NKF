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
                $"Tenant API did not return a connection for tenant '{tenantId}' in context '{_contextName}'. " +
                $"Verify that the tenant has a '{_contextName}' context app and that Tenant API is configured correctly.");
        }

        string requestUri = $"{_tenantApiBaseUrl.TrimEnd('/')}/Tenant?Slug={Uri.EscapeDataString(tenantId)}&ContextName={Uri.EscapeDataString(_contextName)}";
        TenantApiResponse? response = await Client.GetFromJsonAsync<TenantApiResponse>(requestUri);
        if (response is null)
        {
            throw new InvalidOperationException(
                $"Tenant API returned an empty response while resolving tenant '{tenantId}' for context '{_contextName}'.");
        }

        if (!response.IsSucces || response.Value is null)
        {
            string errors = response.Errors is { Count: > 0 }
                ? string.Join("; ", response.Errors)
                : "No error details were returned.";
            throw new InvalidOperationException(
                $"Tenant API could not load connections for context '{_contextName}': {errors}");
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
        catch (HttpRequestException exception)
        {
            throw new InvalidOperationException(
                $"Could not load tenant connections for context '{_contextName}' from Tenant API '{_tenantApiBaseUrl}'.",
                exception);
        }
        catch (TaskCanceledException exception)
        {
            throw new InvalidOperationException(
                $"The request for tenant connections for context '{_contextName}' timed out at Tenant API '{_tenantApiBaseUrl}'.",
                exception);
        }

        if (response is null)
        {
            throw new InvalidOperationException(
                $"Tenant API returned an empty response while listing connections for context '{_contextName}'.");
        }

        if (!response.IsSucces || response.Value is null)
        {
            string errors = response.Errors is { Count: > 0 }
                ? string.Join("; ", response.Errors)
                : "No error details were returned.";
            throw new InvalidOperationException(
                $"Tenant API could not list connections for context '{_contextName}': {errors}");
        }

        foreach (TenantConnection tenant in response.Value)
        {
            _connectionStrings[tenant.Id.ToString()] = tenant.ConnectionString;
        }
    }

    public IReadOnlyList<string> AllActive()
    {
        return _connectionStrings.Values.Distinct().ToArray();
    }

    public IReadOnlyList<Assignment<string>> AllActiveByTenant()
    {
        return _connectionStrings
            .Select(tenant => new Assignment<string>(tenant.Key, tenant.Value))
            .ToArray();
    }

    private sealed record TenantApiResponse(bool IsSucces, TenantConnection? Value, List<string>? Errors);
    private sealed record TenantListApiResponse(bool IsSucces, List<TenantConnection>? Value, List<string>? Errors);
    private sealed record TenantConnection(Guid Id, string ConnectionString, string Name);
}
