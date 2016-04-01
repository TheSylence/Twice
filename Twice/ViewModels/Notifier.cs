using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Twice.Messages;
using Twice.Models.Configuration;
using Twice.ViewModels.Columns.Definitions;
using Twice.ViewModels.Flyouts;
using Twice.ViewModels.Twitter;
using Twice.Views;

namespace Twice.ViewModels
{
	internal class Notifier : INotifier
	{
		public Notifier( IConfig config, IMessenger messenger )
		{
			MessengerInstance = messenger;
			Config = config;

			if( config.Notifications.SoundEnabled && File.Exists( config.Notifications.SoundFileName ) )
			{
				Player = new MediaPlayer();
				Player.Open( new Uri( config.Notifications.SoundFileName ) );
			}

			if( config.Notifications.PopupEnabled )
			{
				PopupNotify = new NotifyIcon();
			}
		}

		public void DisplayMessage( string message, NotificationType type )
		{
			if( !Config.Notifications.ToastsEnabled )
			{
				return;
			}

			var context = new NotificationViewModel( message, type );
			NotifyToast( context );
		}

		public void OnStatus( StatusViewModel status, ColumnNotifications columnSettings )
		{
			if( Config.Notifications.SoundEnabled && columnSettings.Sound )
			{
				NotifySound( status );
			}

			if( Config.Notifications.ToastsEnabled && columnSettings.Toast )
			{
				NotifyToast( status );
			}

			if( Config.Notifications.PopupEnabled && columnSettings.Popup )
			{
				NotifyPopup( status );
			}
		}

		private void NotifyPopup( StatusViewModel status )
		{
		}

		private void NotifySound( StatusViewModel status )
		{
			Player?.Play();
		}

		private void NotifyToast( StatusViewModel status )
		{
			var context = new NotificationViewModel( status );
			NotifyToast( context );
		}

		private void NotifyToast( NotificationViewModel vm )
		{
			DispatcherHelper.CheckBeginInvokeOnUI(
				() => MessengerInstance.Send( new FlyoutMessage( FlyoutNames.NotificationBar, FlyoutAction.Open, vm ) ) );

			Task.Delay( TimeSpan.FromSeconds( 5 ) ).ContinueWith( t =>
			{
				DispatcherHelper.CheckBeginInvokeOnUI( () =>
					MessengerInstance.Send( new FlyoutMessage( FlyoutNames.NotificationBar, FlyoutAction.Close ) ) );
			} );
		}

		private readonly IConfig Config;
		private readonly IMessenger MessengerInstance;
		private readonly MediaPlayer Player;
		private readonly NotifyIcon PopupNotify;
	}
}