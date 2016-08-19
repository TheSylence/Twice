using System.Collections.Generic;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Twice.Models.Configuration
{
	internal class MuteConfig
	{
		public MuteConfig()
		{
			Entries = new List<MuteEntry>();
		}

		public List<MuteEntry> Entries { get; set; }
	}
}