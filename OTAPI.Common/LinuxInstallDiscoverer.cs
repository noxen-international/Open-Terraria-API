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
    public class LinuxInstallDiscoverer : BaseInstallDiscoverer
    {
        public override string[] SearchPaths { get; } = new[]
        {
            "/home/[USER_NAME]/.steam/debian-installation/steamapps/common/Terraria",
            "/home/[USER_NAME]/.steam/steam/steamapps/common/Terraria",
        };

        public override OSPlatform GetClientPlatform() => OSPlatform.Linux;

        public override string GetResource(string fileName, string installPath) => Path.Combine(installPath, fileName);
        public override string GetResourcePath(string installPath) => installPath;

        public override bool IsValidInstallPath(string folder)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return false;

            bool valid = Directory.Exists(folder);

            var startScript = Path.Combine(folder, "Terraria");
            var startBin = Path.Combine(folder, "Terraria.bin.x86_64");
            var assembly = Path.Combine(folder, "Terraria.exe");

            valid &= File.Exists(startScript);
            valid &= File.Exists(startBin);
            valid &= File.Exists(assembly);

            return valid;
        }
    }
}
