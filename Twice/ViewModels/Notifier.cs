using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using GalaSoft.MvvmLight.Messaging;
using Twice.Messages;
using Twice.Models.Columns;
using Twice.Models.Configuration;
using Twice.Utilities.Ui;
using Twice.ViewModels.Flyouts;
using Twice.ViewModels.Twitter;
using Twice.Views;

namespace Twice.ViewModels
{
	internal class Notifier : INotifier
	{
		public Notifier( IConfig config, IMessenger messenger, IDispatcher dispatcher )
		{
			Dispatcher = dispatcher;
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
			Dispatcher.CheckBeginInvokeOnUI(
				() => MessengerInstance.Send( new FlyoutMessage( FlyoutNames.NotificationBar, FlyoutAction.Open, vm ) ) );

			Task.Delay( TimeSpan.FromSeconds( 5 ) ).ContinueWith( t =>
			{
				// TODO: This should be moved into the ViewModel of the Toast
				Dispatcher.CheckBeginInvokeOnUI( () =>
					MessengerInstance.Send( new FlyoutMessage( FlyoutNames.NotificationBar, FlyoutAction.Close ) ) );
			} );
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

		private readonly IConfig Config;
		private readonly IDispatcher Dispatcher;
		private readonly IMessenger MessengerInstance;
		private readonly MediaPlayer Player;
		private readonly NotifyIcon PopupNotify;
	}
}