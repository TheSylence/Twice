using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Twice.Models.Configuration;

namespace Twice.ViewModels.Settings
{
	internal class NotificationSettings : ViewModelBaseEx, INotificationSettings
	{
		public NotificationSettings( IConfig currentConfig )
		{
			AvailableNotifications = new List<NotificationModuleSettings>
			{
				new SoundNotificationSettings( currentConfig ),
				new PopupNotificationSettings( currentConfig ),
				new ToastNotificationSettings( currentConfig )
			};
			foreach( var notifyModule in AvailableNotifications )
			{
				notifyModule.PropertyChanged += NotifyModule_PropertyChanged;
			}

			EnabledNotifications = new ObservableCollection<NotificationModuleSettings>( AvailableNotifications.Where( c => c.Enabled ) );
		}

		public void SaveTo( IConfig config )
		{
			foreach( var notifyModule in AvailableNotifications )
			{
				notifyModule.SaveTo( config );
			}
		}

		private void NotifyModule_PropertyChanged( object sender, System.ComponentModel.PropertyChangedEventArgs e )
		{
			var notifyModule = sender as NotificationModuleSettings;
			if( notifyModule == null )
			{
				return;
			}

			if( nameof( NotificationModuleSettings.Enabled ).Equals( e.PropertyName ) )
			{
				if( notifyModule.Enabled )
				{
					EnabledNotifications.Add( notifyModule );
				}
				else
				{
					EnabledNotifications.Remove( notifyModule );
				}
			}
		}

		public ICollection<NotificationModuleSettings> AvailableNotifications { get; }

		public ICollection<NotificationModuleSettings> EnabledNotifications { get; }
	}
}