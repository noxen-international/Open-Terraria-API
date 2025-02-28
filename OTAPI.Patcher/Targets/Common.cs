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
using System;
using System.Reflection;

namespace OTAPI.Patcher.Targets
{
    [MonoMod.MonoModIgnore]
    public static partial class Common
    {
        public static string GetVersion()
        {
            return typeof(Common).Assembly
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                .InformationalVersion;
        }

        public static void Log(string message)
        {
            Console.WriteLine($"[ModFw] {message}");
        }

        public static string GetCliValue(string key)
        {
            string find = $"-{key}=";
            var match = Array.Find(Environment.GetCommandLineArgs(), x => x.StartsWith(find, StringComparison.CurrentCultureIgnoreCase));
            return match?.Substring(find.Length)?.ToLower();
        }

        public static string GetGitCommitSha()
        {
            var commitSha = Environment.GetEnvironmentVariable("GITHUB_SHA")?.Trim();
            if (commitSha != null && commitSha.Length >= 7)
            {
                return commitSha.Substring(0, 7);
            }
            return null;
        }
    }
}
