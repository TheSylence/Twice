using System;
using System.Diagnostics;
using System.IO;
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

		public string SoundFile
		{
			[DebuggerStepThrough]
			get { return _SoundFile; }
			set
			{
				if( _SoundFile == value )
				{
					return;
				}

				_SoundFile = value;
				RaisePropertyChanged();
			}
		}

		public override string Title => Strings.SoundNotification;
		private readonly MediaPlayer Player;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private string _SoundFile;
	}
}