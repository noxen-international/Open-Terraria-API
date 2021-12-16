using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using ModFramework;
using OTAPI.Patcher.Targets;

namespace OTAPI.Client.Launcher.Patchers
{
    public class TestResolver : AssemblyLoadContext
    {
        private AssemblyDependencyResolver _resolver;
        private Dictionary<AssemblyName, Assembly?> _assemblyCache = new Dictionary<AssemblyName, Assembly?>();

        public List<String> SearchPaths { get; set; } = new List<String>()
        {
            Environment.CurrentDirectory,
            "bin",
        };


        public TestResolver() : base(isCollectible: true)
        {
            var path = Path.Combine(AppContext.BaseDirectory, "OTAPI.Patcher.dll");
            _resolver = new AssemblyDependencyResolver(path);

            // force load our assembly before the module itself loads its own
            this.Load(typeof(ModFramework.Modules.ClearScript.Hooks).Assembly.GetName());
            this.Load(typeof(ModFramework.Modules.CSharp.Hooks).Assembly.GetName());
            this.Load(typeof(ModFramework.Modules.Lua.Hooks).Assembly.GetName());
        }

        protected override Assembly? Load(AssemblyName name)
        {
            if(_assemblyCache.TryGetValue(name, out var assembly))
                return assembly;

            if (name.Name == typeof(ModificationAttribute).Assembly.GetName().Name)
                return typeof(ModificationAttribute).Assembly;
            if (name.Name == typeof(ModFramework.Modules.ClearScript.Hooks).Assembly.GetName().Name)
                return typeof(ModFramework.Modules.ClearScript.Hooks).Assembly;
            if (name.Name == typeof(ModFramework.Modules.CSharp.Hooks).Assembly.GetName().Name)
                return typeof(ModFramework.Modules.ClearScript.Hooks).Assembly;
            if (name.Name == typeof(ModFramework.Modules.ClearScript.Hooks).Assembly.GetName().Name)
                return typeof(ModFramework.Modules.Lua.Hooks).Assembly;
            if (name.Name == typeof(MonoMod.MonoModder).Assembly.GetName().Name)
                return typeof(MonoMod.MonoModder).Assembly;
            if (name.Name == typeof(Mono.Cecil.AssemblyDefinition).Assembly.GetName().Name)
                return typeof(Mono.Cecil.AssemblyDefinition).Assembly;

            Console.WriteLine($"Resolving client patch file: {name}");

            // find via deps.json
            string? assemblyPath = _resolver?.ResolveAssemblyToPath(name);
            if (assemblyPath != null)
            {
                return _assemblyCache[name] = LoadFromAssemblyPath(assemblyPath);
            }

            foreach (var searchPath in SearchPaths)
                foreach (var variant in new[] { ".dll", ".exe" })
                {
                    assemblyPath = name.Name + variant;
                    if (File.Exists(assemblyPath))
                    {
                        var fileinfo = new FileInfo(assemblyPath);
                        return _assemblyCache[name] = LoadFromAssemblyPath(fileinfo.FullName);
                    }

                    assemblyPath = assemblyPath.Replace("NET", ".NET"); // ImgGuiNET = ImgGui.NET
                    if (File.Exists(assemblyPath))
                    {
                        var fileinfo = new FileInfo(assemblyPath);
                        return _assemblyCache[name] = LoadFromAssemblyPath(fileinfo.FullName);
                    }
                }

            return _assemblyCache[name] = null;
        }
    }
}

