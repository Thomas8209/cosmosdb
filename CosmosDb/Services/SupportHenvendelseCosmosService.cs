using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CosmosDb.Configuration;
using CosmosDb.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace CosmosDb.Services;

public interface ISupportHenvendelseService
{
    Task InsertAsync(SupportHenvendelse henvendelse, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SupportHenvendelse>> GetAllAsync(CancellationToken cancellationToken = default);
}

public sealed class SupportHenvendelseCosmosService : ISupportHenvendelseService, IDisposable
{
    private readonly CosmosClient _client;
    private readonly CosmosDbOptions _settings;
    private Container? _container;
    private static readonly TimeZoneInfo DanishTimeZone = ResolveDanishTimeZone();

    public SupportHenvendelseCosmosService(IOptions<CosmosDbOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);
        _settings = options.Value;

        EnsureSetting(_settings.ConnectionString, nameof(CosmosDbOptions.ConnectionString));
        EnsureSetting(_settings.DatabaseName, nameof(CosmosDbOptions.DatabaseName));
        EnsureSetting(_settings.ContainerName, nameof(CosmosDbOptions.ContainerName));

        _client = new CosmosClient(_settings.ConnectionString);
    }

    public async Task InsertAsync(SupportHenvendelse henvendelse, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(henvendelse);

        var danskTid = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, DanishTimeZone);
        henvendelse.OprettetTidspunktDanskTid = danskTid;

        var container = await GetContainerAsync(cancellationToken).ConfigureAwait(false);
        await container.CreateItemAsync(henvendelse, new PartitionKey(henvendelse.Id), cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<SupportHenvendelse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var container = await GetContainerAsync(cancellationToken).ConfigureAwait(false);
        var results = new List<SupportHenvendelse>();

        using var iterator = container.GetItemQueryIterator<SupportHenvendelse>("SELECT * FROM c");

        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync(cancellationToken).ConfigureAwait(false);
            results.AddRange(response);
        }

        return results;
    }

    private async Task<Container> GetContainerAsync(CancellationToken cancellationToken)
    {
        if (_container is not null)
        {
            return _container;
        }

        var databaseResponse = await _client.CreateDatabaseIfNotExistsAsync(_settings.DatabaseName, cancellationToken: cancellationToken).ConfigureAwait(false);
        var containerResponse = await databaseResponse.Database.CreateContainerIfNotExistsAsync(_settings.ContainerName, "/id", cancellationToken: cancellationToken).ConfigureAwait(false);
        _container = containerResponse.Container;

        return _container;
    }

    private static void EnsureSetting(string value, string settingName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"CosmosDb setting '{settingName}' is not configured.");
        }
    }

    private static TimeZoneInfo ResolveDanishTimeZone()
    {
        foreach (var id in new[] { "Europe/Copenhagen", "Central European Standard Time" })
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(id);
            }
            catch (TimeZoneNotFoundException)
            {
                // prøv næste
            }
            catch (InvalidTimeZoneException)
            {
                // prøv næste
            }
        }

        return TimeZoneInfo.Utc;
    }

    public void Dispose() => _client.Dispose();
}
