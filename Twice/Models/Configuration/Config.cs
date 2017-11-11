using System.IO;
using Anotar.NLog;
using Newtonsoft.Json;
using Twice.Utilities;

namespace Twice.Models.Configuration
{
	internal class Config : IConfig
	{
		public Config( string fileName, ISerializer serializer )
		{
			Serializer = serializer;
			bool defaultNeeded = true;
			FileName = fileName;

			if( File.Exists( FileName ) )
			{
				LogTo.Info( $"Trying to load config from {fileName}" );

				string json = File.ReadAllText( FileName );
				if( !string.IsNullOrEmpty( json ) )
				{
					try
					{
						Config tmp = Serializer?.Deserialize<Config>( json );
						if( tmp != null )
						{
							Visual = tmp.Visual;
							General = tmp.General;
							Mute = tmp.Mute;
							Notifications = tmp.Notifications;
							defaultNeeded = false;
						}
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
			LogTo.Info( "No configuration saved. Loading default values" );
			General = new GeneralConfig();
			Visual = new VisualConfig();
			Mute = new MuteConfig();
			Notifications = new NotificationConfig();
		}

		public GeneralConfig General { get; set; }
		public MuteConfig Mute { get; set; }
		public NotificationConfig Notifications { get; set; }

		public void Save()
		{
			string json = Serializer.Serialize( this );
			File.WriteAllText( FileName, json );
		}

		public VisualConfig Visual { get; set; }
		private readonly string FileName;
		private readonly ISerializer Serializer;
	}
}