using System.IO;
using Newtonsoft.Json;

namespace Twice.Models.Configuration
{
	internal class Config : IConfig
	{
		public Config( string fileName )
		{
			bool defaultNeeded = true;
			FileName = fileName;

			if( File.Exists( FileName ) )
			{
				string json = File.ReadAllText( FileName );
				if( !string.IsNullOrEmpty( json ) )
				{
					try
					{
						Config tmp = JsonConvert.DeserializeObject<Config>( json );
						Visual = tmp.Visual;
						General = tmp.General;
						Mute = tmp.Mute;
						Notifications = tmp.Notifications;
						defaultNeeded = false;
					}
					catch( JsonReaderException )
					{
					}
				}
			}

			if( defaultNeeded )
			{
				DefaultConfig();
			}
		}

		private void DefaultConfig()
		{
			General = new GeneralConfig();
			Visual = new VisualConfig();
			Mute = new MuteConfig();
			Notifications = new NotificationConfig();
		}

		public void Save()
		{
			string json = JsonConvert.SerializeObject( this, Formatting.Indented );
			File.WriteAllText( FileName, json );
		}

		public GeneralConfig General { get; set; }
		public MuteConfig Mute { get; set; }
		public NotificationConfig Notifications { get; set; }
		public VisualConfig Visual { get; set; }
		private readonly string FileName;
	}
}