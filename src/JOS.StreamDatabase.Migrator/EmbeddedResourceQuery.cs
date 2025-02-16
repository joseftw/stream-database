using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace JOS.StreamDatabase.Migrator;

public class EmbeddedResourceQuery
{
    private readonly Dictionary<Assembly, string> _assemblyNames;

    public EmbeddedResourceQuery() : this(Array.Empty<Assembly>())
    {
    }

    public EmbeddedResourceQuery(IEnumerable<Assembly> assembliesToPreload)
    {
        _assemblyNames = [];
        foreach (var assembly in assembliesToPreload)
        {
            _assemblyNames.Add(assembly, assembly.GetName().Name!);
        }
    }

    public Stream Read(string resource)
    {
        return Read<EmbeddedResourceQuery>(resource);
    }

    public Stream Read<T>(string resource)
    {
        var assembly = typeof(T).Assembly;
        return ReadInternal(assembly, resource) ??
               throw new Exception($"Failed to find resource '{resource}' in assembly {assembly.GetName()}");
    }

    private Stream? ReadInternal(Assembly assembly, string resource)
    {
        if (!_assemblyNames.ContainsKey(assembly))
        {
            _assemblyNames[assembly] = assembly.GetName().Name!;
        }
        return assembly.GetManifestResourceStream($"{_assemblyNames[assembly]}.{resource}");
    }
}
