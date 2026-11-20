using System.Collections.Concurrent;
using System.Net.Http.Json;
using JasperFx.Descriptors;
using JasperFx.MultiTenancy;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NKF.Domain.SharedKernels.Results.SimpleResults;
using NKP.Services.Tenant.Application.Contracts.Aggregates.Businesses.Responses;

namespace NKF.Persistence.Postgres.Base.Services;

public sealed class TenantConnectionStringSource : ITenantedSource<string>
{
    private static readonly HttpClient Client = new();
    private readonly ConcurrentDictionary<string, string> _connectionStrings = new(StringComparer.OrdinalIgnoreCase);
    private readonly string _tenantApiBaseUrl;
    private readonly string _contextName;
    private readonly string _databaseHostOverride;

    public TenantConnectionStringSource(IConfiguration configuration, string contextName)
    {
        _tenantApiBaseUrl = configuration["Tenant:BaseUrl"]
                            ?? configuration["Tenant__BaseUrl"]
                            ?? "https://localhost:7031";
        _contextName = contextName;
        _databaseHostOverride = configuration["DatabaseHostOverride"] ?? string.Empty;
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

        string requestUri = $"{_tenantApiBaseUrl.TrimEnd('/')}/internal/business/runtime-connection?Slug={Uri.EscapeDataString(tenantId)}&ContextName={Uri.EscapeDataString(_contextName)}";
        SimpleResult<BusinessNameWithConnectionResponse>? response =
            await Client.GetFromJsonAsync<SimpleResult<BusinessNameWithConnectionResponse>>(requestUri);
        if (response is null)
        {
            throw new InvalidOperationException(
                $"Tenant API returned an empty response while resolving tenant '{tenantId}' for context '{_contextName}'.");
        }

        if (!response.IsSuccess || response.Value is null)
        {
            string errors = response.Errors is { Count: > 0 }
                ? string.Join("; ", response.Errors.Select(error => error.Message))
                : "No error details were returned.";
            throw new InvalidOperationException(
                $"Tenant API could not load connections for context '{_contextName}': {errors}");
        }

        string resolvedConnectionString = NormalizeHost(response.Value.ConnectionString);
        _connectionStrings[response.Value.Id.ToString()] = resolvedConnectionString;
        _connectionStrings[tenantId] = resolvedConnectionString;
        return resolvedConnectionString;
    }

    public async Task RefreshAsync()
    {
        string requestUri = $"{_tenantApiBaseUrl.TrimEnd('/')}/internal/business/runtime-connections?ContextName={Uri.EscapeDataString(_contextName)}";
        SimpleResult<List<BusinessNameWithConnectionResponse>>? response;
        try
        {
            response = await Client.GetFromJsonAsync<SimpleResult<List<BusinessNameWithConnectionResponse>>>(requestUri);
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

        if (!response.IsSuccess || response.Value is null)
        {
            string errors = response.Errors is { Count: > 0 }
                ? string.Join("; ", response.Errors.Select(error => error.Message))
                : "No error details were returned.";
            throw new InvalidOperationException(
                $"Tenant API could not list connections for context '{_contextName}': {errors}");
        }

        foreach (BusinessNameWithConnectionResponse tenant in response.Value)
        {
            _connectionStrings[tenant.Id.ToString()] = NormalizeHost(tenant.ConnectionString);
        }
    }

    private string NormalizeHost(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(_databaseHostOverride))
        {
            return connectionString;
        }

        NpgsqlConnectionStringBuilder builder = new(connectionString);
        if (string.Equals(builder.Host, "localhost", StringComparison.OrdinalIgnoreCase)
            || string.Equals(builder.Host, "127.0.0.1", StringComparison.OrdinalIgnoreCase))
        {
            builder.Host = _databaseHostOverride;
        }

        return builder.ConnectionString ?? connectionString;
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

}
