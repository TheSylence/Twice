using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Media;
using Twice.Models.Configuration;
using Twice.Resources;
using Twice.Utilities;

namespace Twice.ViewModels.Settings
{
	internal class SoundNotificationSettings : NotificationModuleSettings
	{
		public SoundNotificationSettings( IConfig config )
		{
			Enabled = config.Notifications.SoundEnabled;
			SoundFile = config.Notifications.SoundFileName;

			Player = new MediaPlayer();

			FileTypeFilter = GetSupportedFileTypes();
		}

		public override void SaveTo( IConfig config )
		{
			config.Notifications.SoundEnabled = Enabled;
			config.Notifications.SoundFileName = SoundFile;
		}

		protected override void ExecutePreviewCommand()
		{
			var file = File.Exists( SoundFile ) ? SoundFile : ResourceHelper.GetDefaultNotificationSound();

			Player.Open( new Uri( file ) );
			Player.Play();
		}

		private string GetSupportedFileTypes()
		{ 
			var extensions = new[]
			{
				"asf","wma","m4a","aac","adt","adts",
				"cda","mp2","mp3","mid","midi","aif",
				"aifc","aiff","wav","au","snd"
			};

			var display = Strings.SupportedMediaTypes;
			var joined = string.Join( ";", extensions.OrderBy( x => x ).Select( e => $"*.{e}" ) );
			return $"{display}|{joined}";
		}

		public string FileTypeFilter { get; }

		public string SoundFile { get; set; }

		public override string Title => Strings.SoundNotification;
		private readonly MediaPlayer Player;
	}
}