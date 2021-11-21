﻿/*
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
using System.IO;
using System.Runtime.InteropServices;

namespace OTAPI.Common
{
    public class MacOSInstallDiscoverer : BaseInstallDiscoverer
    {
        public override string[] SearchPaths { get; } = new[]
        {
            "/Users/[USER_NAME]/Library/Application Support/Steam/steamapps/common/Terraria/Terraria.app/Contents/",
            "/Applications/Terraria.app/Contents/",
        };

        public override OSPlatform GetClientPlatform() => OSPlatform.OSX;

        public override string GetResource(string fileName, string installPath) => Path.Combine(installPath, "Resources", fileName);
        public override string GetResourcePath(string installPath) => Path.Combine(installPath, "Resources");

        public override bool IsValidInstallPath(string folder)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return false;

            bool valid = Directory.Exists(folder);

            var macOS = Path.Combine(folder, "MacOS");
            var resources = Path.Combine(folder, "Resources");

            var startScript = Path.Combine(macOS, "Terraria");
            var startBin = Path.Combine(macOS, "Terraria.bin.osx");
            var assembly = Path.Combine(resources, "Terraria.exe");

            valid &= Directory.Exists(macOS);
            valid &= Directory.Exists(resources);

            valid &= File.Exists(startScript);
            valid &= File.Exists(startBin);
            valid &= File.Exists(assembly);

            return valid;
        }
    }
}
