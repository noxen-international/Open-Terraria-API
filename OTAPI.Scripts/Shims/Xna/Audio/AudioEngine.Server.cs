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
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace Microsoft.Xna.Framework.Audio
{
    public class AudioEngine
    {
        public AudioEngine(string settingsFile) { }

		public void Update() { }
	}

	[Serializable]
	public sealed class NoAudioHardwareException : ExternalException
	{
		public NoAudioHardwareException()
		{
		}

		public NoAudioHardwareException(string message)
			: base(message)
		{
		}

		public NoAudioHardwareException(string message, Exception inner)
			: base(message, inner)
		{
		}

		private NoAudioHardwareException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}

}