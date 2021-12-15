using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using OTAPI.Patcher.Targets;

namespace OTAPI.Client.Launcher.Patchers
{
    public class TestResolver : AssemblyLoadContext
    {
        private AssemblyDependencyResolver _resolver;


        public TestResolver() : base(isCollectible: true)
        {
            var path = Path.Combine(AppContext.BaseDirectory, "OTAPI.Patcher.dll");
            _resolver = new AssemblyDependencyResolver(path);
        }

        protected override Assembly Load(AssemblyName name)
        {
            //Console.WriteLine($"Lookin for {name}");
            string assemblyPath = _resolver.ResolveAssemblyToPath(name);
            if (assemblyPath != null)
            {
                return LoadFromAssemblyPath(assemblyPath);
            }

            return null;
        }
    }

    //public class ClientPatcher : AssemblyLoadContext
    //{
    //    private AssemblyDependencyResolver _resolver;

    //    public virtual string EntryPoint { get; set; }

    //    public ClientPatcher(string? entryPoint = null) : base(isCollectible: true)
    //    {
    //        EntryPoint = entryPoint ?? Path.Combine(AppContext.BaseDirectory, "OTAPI.Patcher.dll");
    //        _resolver = new AssemblyDependencyResolver(EntryPoint);
    //    }

    //    protected override Assembly Load(AssemblyName name)
    //    {
    //        string assemblyPath = _resolver.ResolveAssemblyToPath(name);
    //        if (assemblyPath != null)
    //        {
    //            return LoadFromAssemblyPath(assemblyPath);
    //        }

    //        return null;
    //    }

    //    public void Patch(string installPath, EventHandler<StatusUpdateArgs> onUpdate)
    //    {
    //        var asm = this.LoadFromAssemblyPath(EntryPoint);
    //        var targetClass = asm.ExportedTypes.Single(t => t.FullName == "OTAPI.Patcher.Targets.OTAPIClientLightweightTarget");
    //        dynamic target = Activator.CreateInstance(targetClass);
    //        target.InstallPath = installPath;

    //        target.StatusUpdate += (sender, e) => onUpdate(sender, e);
    //        target.Patch();

    //        target.StatusUpdate -= onUpdate;
    //        //Context.InstallStatus = "Patching completed, installing to existing installation...";

    //        //Context.InstallPath.Target.StatusUpdate += (sender, e) => Context.InstallStatus = e.Text;
    //        //Context.InstallPath.Target.Install(Context.InstallPath.Path);

    //        Console.WriteLine("Loaded asdasdas");
    //        //var args = new object[] { new [] { "--patch-client", installPath } };
    //        //asm.EntryPoint.Invoke(null, args);
    //    }

    //    static string? GetInstallPath()
    //    {
    //        var args = Environment.GetCommandLineArgs();
    //        var ix = Array.FindIndex(args, i => i.Equals("--patch-client"));
    //        if(ix > -1)
    //        {
    //            var pathIx = ix + 1;
    //            if (pathIx < args.Length)
    //                return args[pathIx];
    //        }
    //        return null;
    //    }

    //    internal static bool TryProcess()
    //    {
    //        var installPath = GetInstallPath();
    //        if (!String.IsNullOrWhiteSpace(installPath) && Directory.Exists(installPath))
    //        {
    //            var target = new OTAPIClientLightweightTarget();
    //            target.InstallPath = installPath;

    //            //target.StatusUpdate += (sender, e) => Context.InstallStatus = e.Text;
    //            target.Patch();
    //            //Context.InstallStatus = "Patching completed, installing to existing installation...";

    //            //Context.InstallPath.Target.StatusUpdate += (sender, e) => Context.InstallStatus = e.Text;
    //            //Context.InstallPath.Target.Install(Context.InstallPath.Path);

    //            return true;
    //        }
    //        return false;
    //    }
    //}
}

