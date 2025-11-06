using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleSnippets;

internal static class ReflectionShowcase
{
    public static async Task RunAsync()
    {
        Console.WriteLine("== Reflection showcase ==");

        var pluginDescriptors = ReflectionPluginCatalog.Discover();
        Console.WriteLine("Discovered plugins:");
        foreach (var descriptor in pluginDescriptors)
        {
            Console.WriteLine($" - {descriptor.Attribute?.Order:D2} :: {descriptor.Attribute?.Description ?? descriptor.Type.Name}");
        }

        var executor = ReflectionPluginCatalog.BuildExecutor(pluginDescriptors);
        var payload = "hello from MAUI";
        var response = await executor(payload, CancellationToken.None);
        Console.WriteLine($"Invoker response: {response}");

        var location = Assembly.GetExecutingAssembly().Location;
        if (!string.IsNullOrWhiteSpace(location) && File.Exists(location))
        {
            using var sandbox = new PluginSandbox(location);
            var summaries = sandbox.LoadPluginDescriptors();
            Console.WriteLine($"Sandbox inspected {summaries.Count} plugin types via collectible AssemblyLoadContext.");
            foreach (var summary in summaries)
            {
                Console.WriteLine($"   • {summary.TypeName} (Order: {summary.Order}, Description: {summary.Description})");
            }
        }
        else
        {
            Console.WriteLine("Assembly location unavailable; skipping sandbox demonstration.");
        }
    }
}

internal static class ReflectionPluginCatalog
{
    public static IReadOnlyList<ReflectionPluginDescriptor> Discover()
    {
        var pluginTypes = typeof(IReflectionPlugin).Assembly
            .GetTypes()
            .Where(t => !t.IsAbstract && typeof(IReflectionPlugin).IsAssignableFrom(t))
            .Select(t => new ReflectionPluginDescriptor(t, t.GetCustomAttribute<ReflectionPluginAttribute>()))
            .OrderBy(t => t.Attribute?.Order ?? int.MaxValue)
            .ToList();

        return pluginTypes;
    }

    public static Func<string, CancellationToken, Task<string>> BuildExecutor(
        IReadOnlyList<ReflectionPluginDescriptor> descriptors)
    {
        var executeMethod = typeof(IReflectionPlugin).GetMethod(nameof(IReflectionPlugin.ExecuteAsync))!;
        var invoker = executeMethod.CreateDelegate<Func<IReflectionPlugin, string, CancellationToken, Task<string>>>();

        return async (input, token) =>
        {
            foreach (var descriptor in descriptors)
            {
                var plugin = (IReflectionPlugin)Activator.CreateInstance(descriptor.Type)!;
                var loggingProxy = PluginLoggingProxy.Create(plugin, message =>
                    Console.WriteLine($"[{plugin.Name}] {message}"));
                input = await invoker(loggingProxy, input, token).ConfigureAwait(false);
            }

            return input;
        };
    }
}

internal sealed record ReflectionPluginDescriptor(Type Type, ReflectionPluginAttribute? Attribute);

[AttributeUsage(AttributeTargets.Class)]
internal sealed class ReflectionPluginAttribute : Attribute
{
    public ReflectionPluginAttribute(string description) => Description = description;

    public string Description { get; }

    public int Order { get; init; }
}

internal interface IReflectionPlugin
{
    string Name { get; }

    Task<string> ExecuteAsync(string payload, CancellationToken token);
}

[ReflectionPlugin("Transforms text to uppercase", Order = 0)]
internal sealed class UppercasePlugin : IReflectionPlugin
{
    public string Name => "Uppercase";

    public Task<string> ExecuteAsync(string payload, CancellationToken token)
        => Task.FromResult(payload.ToUpperInvariant());
}

[ReflectionPlugin("Appends execution metadata", Order = 1)]
internal sealed class MetadataPlugin : IReflectionPlugin
{
    public string Name => "Metadata";

    public Task<string> ExecuteAsync(string payload, CancellationToken token)
    {
        var timestamp = DateTimeOffset.UtcNow.ToString("u");
        var caller = MethodBase.GetCurrentMethod()?.DeclaringType?.Name ?? nameof(MetadataPlugin);
        return Task.FromResult($"{payload} [{caller} at {timestamp}]");
    }
}

internal class PluginLoggingProxy : DispatchProxy
{
    private IReflectionPlugin _decorated = default!;
    private Action<string> _log = default!;

    public static IReflectionPlugin Create(IReflectionPlugin decorated, Action<string> log)
    {
        var proxy = Create<IReflectionPlugin, PluginLoggingProxy>();
        var wrapper = (PluginLoggingProxy)(object)proxy;
        wrapper._decorated = decorated;
        wrapper._log = log;
        return proxy;
    }

    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        if (targetMethod is null)
        {
            throw new ArgumentNullException(nameof(targetMethod));
        }

        var stopwatch = Stopwatch.StartNew();
        try
        {
            var result = targetMethod.Invoke(_decorated, args ?? Array.Empty<object?>());
            if (result is Task task)
            {
                if (result is Task<string> stringTask)
                {
                    return InterceptAsync(stringTask, targetMethod, stopwatch);
                }

                return InterceptAsync(task, targetMethod, stopwatch);
            }

            stopwatch.Stop();
            _log($"Method {targetMethod.Name} completed in {stopwatch.ElapsedMilliseconds} ms");
            return result;
        }
        catch (TargetInvocationException ex) when (ex.InnerException is not null)
        {
            throw ex.InnerException;
        }
    }

    private async Task InterceptAsync(Task task, MethodInfo method, Stopwatch stopwatch)
    {
        try
        {
            await task.ConfigureAwait(false);
        }
        finally
        {
            stopwatch.Stop();
            _log($"Method {method.Name} completed in {stopwatch.ElapsedMilliseconds} ms");
        }
    }

    private async Task<string> InterceptAsync(Task<string> task, MethodInfo method, Stopwatch stopwatch)
    {
        try
        {
            return await task.ConfigureAwait(false);
        }
        finally
        {
            stopwatch.Stop();
            _log($"Method {method.Name} completed in {stopwatch.ElapsedMilliseconds} ms");
        }
    }
}

internal sealed class PluginSandbox : IDisposable
{
    private readonly CollectiblePluginLoadContext _context;
    private readonly string _assemblyPath;
    private Assembly? _loadedAssembly;
    private bool _disposed;

    public PluginSandbox(string assemblyPath)
    {
        _assemblyPath = assemblyPath;
        _context = new CollectiblePluginLoadContext(assemblyPath);
    }

    public IReadOnlyList<PluginSummary> LoadPluginDescriptors()
    {
        _loadedAssembly ??= _context.LoadFromAssemblyPath(_assemblyPath);
        var pluginInterface = typeof(IReflectionPlugin).FullName;

        var descriptors = _loadedAssembly
            .GetTypes()
            .Where(t => !t.IsAbstract && t.IsClass && ImplementsPluginContract(t, pluginInterface))
            .Select(t =>
            {
                var attribute = t.GetCustomAttributesData()
                    .FirstOrDefault(a => a.AttributeType.FullName == typeof(ReflectionPluginAttribute).FullName);

                var description = attribute?.ConstructorArguments.FirstOrDefault().Value as string;
                var orderArg = attribute?.NamedArguments.FirstOrDefault(a => a.MemberName == nameof(ReflectionPluginAttribute.Order));
                var order = orderArg?.TypedValue.Value is int value ? value : int.MaxValue;

                return new PluginSummary(t.FullName ?? t.Name, description ?? string.Empty, order);
            })
            .ToList();

        return descriptors;
    }

    private static bool ImplementsPluginContract(Type candidate, string? contractFullName)
        => candidate.GetInterfaces().Any(i => i.FullName == contractFullName);

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _context.Unload();
        GC.Collect();
        GC.WaitForPendingFinalizers();
        _disposed = true;
    }

    private sealed class CollectiblePluginLoadContext : AssemblyLoadContext
    {
        private readonly AssemblyDependencyResolver _resolver;

        public CollectiblePluginLoadContext(string assemblyPath)
            : base(isCollectible: true)
        {
            _resolver = new AssemblyDependencyResolver(assemblyPath);
        }

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            var path = _resolver.ResolveAssemblyToPath(assemblyName);
            return path is null ? null : LoadFromAssemblyPath(path);
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            var path = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            return path is null ? IntPtr.Zero : LoadUnmanagedDllFromPath(path);
        }
    }
}

internal sealed record PluginSummary(string TypeName, string Description, int Order);
