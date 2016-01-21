using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Twice.Models.Configuration;

namespace Twice.ViewModels.Settings
{
	interface INotificationSettings : ISettingsSection
	{
		ICollection<NotificationModuleSettings> AvailableNotifications { get; }
		Corner DisplayCorner { get; set; }
		int DisplayIndex { get; set; }
		ICollection<NotificationModuleSettings> EnabledNotifications { get; }
		bool EnablePopups { get; set; }
		bool EnableSounds { get; set; }
		bool EnableToasts { get; set; }
		string SoundFileName { get; set; }
	}

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
				notifyModule.SaveTo( config);
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

		public Corner DisplayCorner
		{
			[System.Diagnostics.DebuggerStepThrough] get { return _DisplayCorner; }
			set
			{
				if( _DisplayCorner == value )
				{
					return;
				}

				_DisplayCorner = value;
				RaisePropertyChanged();
			}
		}

		public int DisplayIndex
		{
			[System.Diagnostics.DebuggerStepThrough] get { return _DisplayIndex; }
			set
			{
				if( _DisplayIndex == value )
				{
					return;
				}

				_DisplayIndex = value;
				RaisePropertyChanged();
			}
		}

		public ICollection<NotificationModuleSettings> EnabledNotifications { get; }

		public bool EnablePopups
		{
			[System.Diagnostics.DebuggerStepThrough] get { return _EnablePopups; }
			set
			{
				if( _EnablePopups == value )
				{
					return;
				}

				_EnablePopups = value;
				RaisePropertyChanged();
			}
		}

		public bool EnableSounds
		{
			[System.Diagnostics.DebuggerStepThrough] get { return _EnableSounds; }
			set
			{
				if( _EnableSounds == value )
				{
					return;
				}

				_EnableSounds = value;
				RaisePropertyChanged();
			}
		}

		public bool EnableToasts
		{
			[System.Diagnostics.DebuggerStepThrough] get { return _EnableToasts; }
			set
			{
				if( _EnableToasts == value )
				{
					return;
				}

				_EnableToasts = value;
				RaisePropertyChanged();
			}
		}

		public string SoundFileName
		{
			[System.Diagnostics.DebuggerStepThrough] get { return _SoundFileName; }
			set
			{
				if( _SoundFileName == value )
				{
					return;
				}

				_SoundFileName = value;
				RaisePropertyChanged();
			}
		}

		[System.Diagnostics.DebuggerBrowsable( System.Diagnostics.DebuggerBrowsableState.Never )] private Corner _DisplayCorner;

		[System.Diagnostics.DebuggerBrowsable( System.Diagnostics.DebuggerBrowsableState.Never )] private int _DisplayIndex;

		[System.Diagnostics.DebuggerBrowsable( System.Diagnostics.DebuggerBrowsableState.Never )] private bool _EnablePopups;

		[System.Diagnostics.DebuggerBrowsable( System.Diagnostics.DebuggerBrowsableState.Never )] private bool _EnableSounds;

		[System.Diagnostics.DebuggerBrowsable( System.Diagnostics.DebuggerBrowsableState.Never )] private bool _EnableToasts;

		[System.Diagnostics.DebuggerBrowsable( System.Diagnostics.DebuggerBrowsableState.Never )] private string _SoundFileName;
	}
}