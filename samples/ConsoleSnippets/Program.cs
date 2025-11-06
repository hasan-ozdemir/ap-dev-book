using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConsoleSnippets
{
    /// <summary>
    /// Demonstrates C# 13 (preview) features and modern language keywords in a single-file console app.
    /// </summary>
    internal sealed class Program
    {
        private static async Task Main()
        {
            Console.WriteLine("Prodyum C# keyword sampler");

            // Using declarations
            using var scope = new SampleScope("Primary scope");

            // Pattern matching with list patterns
            var apiLevels = new[] { 35, 34, 33 };
            if (apiLevels is [>= 35, ..])
            {
                Console.WriteLine("Ready for Google Play's 2025 API level requirement.");
            }

            // switch expressions with relational and logical patterns
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
            var message = environment switch
            {
                "Development" => "Developer shortcuts enabled.",
                "Staging" => "Running rehearsals before go-live.",
                _ => "Serving end users."
            };
            Console.WriteLine(message);

            // ref locals and scoped keyword (C# 12/13)
            scoped Span<int> numbers = stackalloc int[] { 1, 2, 3, 4, 5 };
            ref readonly var max = ref numbers[^1];
            Console.WriteLine($"Highest seed value: {max}");

            // interpolated string handlers and raw string literals
            var overview = $$"""
                {
                  "app": "Prodyum.TodoApp",
                  "targetFrameworks": ["net9.0-android", "net9.0-ios", "net9.0-windows10.0.19041.0"],
                  "features": ["NativeAOT", "HybridWebView"]
                }
                """;
            Console.WriteLine($"Deployment manifest:\n{overview}");

            // async streams
            await foreach (var checkpoint in GenerateCheckpoints())
            {
                Console.WriteLine($"Checkpoint: {checkpoint}");
            }

            await AsyncShowcase.RunAsyncDemosAsync();
            await ObjectModelShowcase.RunAsync();
            await ReflectionShowcase.RunAsync();

            Console.WriteLine("Sample complete.");
        }

        private static async IAsyncEnumerable<string> GenerateCheckpoints()
        {
            yield return "Provision Azure resources";
            await Task.Delay(50);
            yield return "Configure CI pipeline";
            await Task.Delay(50);
            yield return "Submit to app stores";
        }

        private sealed class SampleScope : IDisposable
        {
            private readonly string _name;
            private bool _disposed;

            public SampleScope(string name) => _name = name;

            public void Dispose()
            {
                if (_disposed)
                {
                    return;
                }

                Console.WriteLine($"Disposing {_name}");
                _disposed = true;
            }
        }
    }
}

