using System.Collections.Generic;

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