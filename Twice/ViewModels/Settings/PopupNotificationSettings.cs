using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Twice.Models.Configuration;
using Twice.Resources;
using Twice.Utilities.Os;

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
			SelectedDisplay = config.Notifications.PopupDisplay;
		}

		public override void SaveTo( IConfig config )
		{
			config.Notifications.PopupEnabled = Enabled;
			config.Notifications.PopupDisplayCorner = SelectedCorner;
			config.Notifications.PopupDisplay = SelectedDisplay;
		}

		private static IEnumerable<ValueDescription<string>> ListDisplays()
		{
			return DisplayHelper.GetAvailableDisplays().Select( ( kvp ) => new ValueDescription<string>( kvp.Key, kvp.Value ) );
		}

		public ICollection<ValueDescription<Corner>> AvailableCorners { get; }
		public ICollection<ValueDescription<string>> AvailableDisplays { get; }

		public Corner SelectedCorner
		{
			[DebuggerStepThrough]
			get { return _SelectedCorner; }
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

		public string SelectedDisplay
		{
			[DebuggerStepThrough]
			get { return _SelectedDisplay; }
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

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private Corner _SelectedCorner;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private string _SelectedDisplay;
	}
}