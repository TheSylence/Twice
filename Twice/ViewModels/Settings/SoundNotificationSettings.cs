using System.Diagnostics;
using Twice.Models.Configuration;
using Twice.Resources;

namespace Twice.ViewModels.Settings
{
	internal class SoundNotificationSettings : NotificationModuleSettings
	{
		public SoundNotificationSettings( IConfig config )
		{
			Enabled = config.Notifications.SoundEnabled;
			SoundFile = config.Notifications.SoundFileName;
		}

		public override void SaveTo( IConfig config )
		{
			config.Notifications.SoundEnabled = Enabled;
			config.Notifications.SoundFileName = SoundFile;
		}

		public string SoundFile
		{
			[DebuggerStepThrough] get { return _SoundFile; }
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

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private string _SoundFile;
	}
}