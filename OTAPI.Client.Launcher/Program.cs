/*
Copyright (C) 2020 DeathCradle

This file is part of Open Terraria API v3 (OTAPI)

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program. If not, see <http://www.gnu.org/licenses/>.
*/
using Avalonia;
using Projektanker.Icons.Avalonia;
using Projektanker.Icons.Avalonia.FontAwesome;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Xilium.CefGlue;

namespace OTAPI.Client.Launcher
{
    class Program
    {
        public static string LaunchFolder { get; set; } = Environment.CurrentDirectory;
        public static string? LaunchID { get; set; }
        public static Targets.IPlatformTarget[] Targets = new Targets.IPlatformTarget[]
        {
            new Targets.MacOSPlatformTarget(),
            new Targets.WindowsPlatformTarget(),
            new Targets.LinuxPlatformTarget(),
        };

        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static void Main(string[] args)
        {
            // FNA added their own native resolver...which doesn't work (or their libs are not correct either)
            // this hack here forces their resolver to not be set, allowing us to configure our own
            // which scans the right folders.
            if (File.Exists("FNA.dll.config"))
                File.Delete("FNA.dll.config");

            // if launching from osx bundle it launches at /
            // we need it to be in MacOS
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                var first = Path.GetDirectoryName(Environment.GetCommandLineArgs().FirstOrDefault());
                if (first is not null && Directory.Exists(first))
                    Environment.CurrentDirectory = first;
            }

            // start the launcher, then OTAPI if requested
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);

            if (LaunchID == "OTAPI")
                Actions.OTAPI.Launch(args);

            else if (LaunchID == "VANILLA")
                Actions.Vanilla.Launch(LaunchFolder, args);
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .AfterSetup(AfterSetupCallback)
                .UsePlatformDetect()
                .LogToTrace();

        // Called after setup
        private static void AfterSetupCallback(AppBuilder appBuilder)
        {
            // Register icon provider(s)
            IconProvider.Register<FontAwesomeIconProvider>();
        }
    }
}
