using Newtonsoft.Json;
using System.IO;

namespace Twice.Models.Configuration
{
	internal interface IConfig
	{
		void Save();

		GeneralConfig General { get; }
		VisualConfig Visual { get; }
		MuteConfig Mute { get; }
		NotificationConfig Notifications { get; }
	}

	internal class Config : IConfig
	{
		public Config( string fileName )
		{
			FileName = fileName;

			if( File.Exists( FileName ) )
			{
				string json = File.ReadAllText( FileName );

				Config tmp = JsonConvert.DeserializeObject<Config>( json );
				Visual = tmp.Visual;
				General = tmp.General;
				Mute = tmp.Mute;
				Notifications = tmp.Notifications;
			}
			else
			{
				General = new GeneralConfig();
				Visual = new VisualConfig();
				Mute = new MuteConfig();
				Notifications = new NotificationConfig();
			}
		}

		public void Save()
		{
			string json = JsonConvert.SerializeObject( this, Formatting.Indented );
			File.WriteAllText( FileName, json );
		}

		public GeneralConfig General { get; set; }
		public VisualConfig Visual { get; set; }
		public MuteConfig Mute { get; set; }
		public NotificationConfig Notifications { get; set; }

		private readonly string FileName;
	}
}