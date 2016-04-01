using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Twice.Models.Configuration;
using Twice.Resources;

namespace Twice.ViewModels.Settings
{
	internal class PopupNotificationSettings : NotificationModuleSettings
	{
		public PopupNotificationSettings( IConfig config )
		{
			AvailableCorners = ValueDescription<Corner>.GetValues( true ).ToList();
			AvailableDisplays = ListDisplays().ToList();

			Enabled = config.Notifications.PopupEnabled;
			SelectedCorner = config.Notifications.PopupDisplayCorner;
			SelectedDisplay = config.Notifications.PopupDisplayIndex;
		}

		public override void SaveTo( IConfig config )
		{
			config.Notifications.PopupEnabled = Enabled;
			config.Notifications.PopupDisplayCorner = SelectedCorner;
			config.Notifications.PopupDisplayIndex = SelectedDisplay;
		}

		private static IEnumerable<ValueDescription<int>> ListDisplays()
		{
			return Screen.AllScreens.Select( ( t, i ) => new ValueDescription<int>( i, t.DeviceName ) );
		}

		public ICollection<ValueDescription<Corner>> AvailableCorners { get; }
		public ICollection<ValueDescription<int>> AvailableDisplays { get; }

		public Corner SelectedCorner
		{
			[System.Diagnostics.DebuggerStepThrough] get { return _SelectedCorner; }
			set
			{
				if( _SelectedCorner == value )
				{
					return;
				}

				_SelectedCorner = value;
				RaisePropertyChanged();
			}
		}

		public int SelectedDisplay
		{
			[System.Diagnostics.DebuggerStepThrough] get { return _SelectedDisplay; }
			set
			{
				if( _SelectedDisplay == value )
				{
					return;
				}

				_SelectedDisplay = value;
				RaisePropertyChanged();
			}
		}

		public override string Title => Strings.PopupNotification;

		[System.Diagnostics.DebuggerBrowsable( System.Diagnostics.DebuggerBrowsableState.Never )] private Corner _SelectedCorner;

		[System.Diagnostics.DebuggerBrowsable( System.Diagnostics.DebuggerBrowsableState.Never )] private int _SelectedDisplay;
	}
}