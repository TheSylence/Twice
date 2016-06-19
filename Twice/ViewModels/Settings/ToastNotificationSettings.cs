using System.Diagnostics;
using Twice.Models.Configuration;
using Twice.Resources;

namespace Twice.ViewModels.Settings
{
	internal class ToastNotificationSettings : NotificationModuleSettings
	{
		public ToastNotificationSettings( IConfig config, INotifier notifier )
		{
			Enabled = config.Notifications.ToastsEnabled;
			Top = config.Notifications.ToastsTop;
			Notifier = notifier;
		}

		public override void SaveTo( IConfig config )
		{
			config.Notifications.ToastsEnabled = Enabled;
			config.Notifications.ToastsTop = Top;
		}

		protected override void ExecutePreviewCommand()
		{
			Notifier.PreviewInAppNotification( Strings.TestNotification, Top );
		}

		public override string Title => Strings.InAppNotification;

		public bool Top
		{
			[DebuggerStepThrough] get { return _Top; }
			set
			{
				if( _Top == value )
				{
					return;
				}

				_Top = value;
				RaisePropertyChanged();
			}
		}

		private readonly INotifier Notifier;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private bool _Top;
	}
}