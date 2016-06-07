using GalaSoft.MvvmLight.Messaging;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
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

		public void DisplayMessage( string message, NotificationType type )
		{
			if( !Config.Notifications.ToastsEnabled )
			{
				return;
			}

			var context = new NotificationViewModel( message, type );
			NotifyToast( context );
		}

		public void OnItem( ColumnItem item, ColumnNotifications columnSettings )
		{
			if( Config.Notifications.SoundEnabled && columnSettings.Sound )
			{
				NotifySound( item );
			}

			if( Config.Notifications.ToastsEnabled && columnSettings.Toast )
			{
				NotifyToast( item );
			}

			if( Config.Notifications.PopupEnabled && columnSettings.Popup )
			{
				NotifyPopup( item );
			}
		}

		private void NotifyPopup( ColumnItem item )
		{
		}

		private void NotifySound( ColumnItem item )
		{
			Player?.Play();
		}

		private void NotifyToast( ColumnItem item )
		{
			var context = new NotificationViewModel( item );
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

		private readonly IConfig Config;
		private readonly IDispatcher Dispatcher;
		private readonly IMessenger MessengerInstance;
		private readonly MediaPlayer Player;
		private readonly NotifyIcon PopupNotify;
	}
}