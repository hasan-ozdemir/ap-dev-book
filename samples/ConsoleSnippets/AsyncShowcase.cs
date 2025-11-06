using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleSnippets;

internal static class AsyncShowcase
{
    private static readonly ConcurrentDictionary<string, string> Cache = new(StringComparer.OrdinalIgnoreCase);
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> InFlight = new();
    private static readonly FakeCatalogClient Catalog = new();

    public static async Task RunAsyncDemosAsync()
    {
        Console.WriteLine("== Async showcase ==");

        const string sku = "PRO-42";
        var first = await FetchProductAsync(sku, CancellationToken.None);
        Console.WriteLine($"Initial lookup ({sku}): {first}");

        var second = await FetchProductAsync(sku, CancellationToken.None);
        Console.WriteLine($"Cached lookup ({sku}): {second}");

        var rates = await FetchRatesAsync(new[] { "USD/EUR", "USD/GBP", "USD/TRY" }, CancellationToken.None);
        foreach (var (symbol, price) in rates)
        {
            Console.WriteLine($"Rate {symbol}: {price}");
        }

        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(220));
        try
        {
            await foreach (var reading in StreamTelemetryAsync(cts.Token))
            {
                Console.WriteLine($"Telemetry reading: {reading:F1}");
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Telemetry stream cancelled.");
        }
    }

    public static ValueTask<string> FetchProductAsync(string sku, CancellationToken token)
    {
        if (Cache.TryGetValue(sku, out var cached))
        {
            return ValueTask.FromResult(cached);
        }

        var gate = InFlight.GetOrAdd(sku, static _ => new SemaphoreSlim(1, 1));
        return FetchAndCacheAsync(sku, gate, token);
    }

    private static async ValueTask<string> FetchAndCacheAsync(string sku, SemaphoreSlim gate, CancellationToken token)
    {
        await gate.WaitAsync(token).ConfigureAwait(false);
        try
        {
            if (Cache.TryGetValue(sku, out var cached))
            {
                return cached;
            }

            var record = await Catalog.LookupAsync(sku, token).ConfigureAwait(false);
            Cache[sku] = record;
            return record;
        }
        finally
        {
            gate.Release();
            InFlight.TryRemove(sku, out _);
        }
    }

    public static async Task<IDictionary<string, decimal>> FetchRatesAsync(IEnumerable<string> symbols, CancellationToken token)
    {
        var lookups = symbols.Select(symbol => Catalog.GetQuoteAsync(symbol, token)).ToArray();
        await Task.WhenAll(lookups).ConfigureAwait(false);
        return lookups.ToDictionary(task => task.Result.symbol, task => task.Result.price);
    }

    public static async IAsyncEnumerable<double> StreamTelemetryAsync(
        [EnumeratorCancellation] CancellationToken token = default)
    {
        while (true)
        {
            token.ThrowIfCancellationRequested();
            await Task.Delay(TimeSpan.FromMilliseconds(70), token).ConfigureAwait(false);
            yield return Math.Round(Random.Shared.NextDouble() * 100, 2);
        }
    }

    private sealed class FakeCatalogClient
    {
        public async Task<string> LookupAsync(string sku, CancellationToken token)
        {
            await Task.Delay(100, token).ConfigureAwait(false);
            return $"Product {sku} (generated {DateTimeOffset.UtcNow:O})";
        }

        public async Task<(string symbol, decimal price)> GetQuoteAsync(string symbol, CancellationToken token)
        {
            await Task.Delay(60, token).ConfigureAwait(false);
            var price = Math.Round((decimal)(Random.Shared.NextDouble() * 10) + 1m, 4);
            return (symbol, price);
        }
    }
}
