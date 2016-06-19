using System;
using System.IO;
using System.Windows.Forms;
using System.Windows.Media;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using GalaSoft.MvvmLight.Messaging;
using NotificationsExtensions;
using NotificationsExtensions.Toasts;
using Twice.Models.Columns;
using Twice.Models.Configuration;
using Twice.Utilities;
using Twice.Utilities.Ui;
using Twice.ViewModels.Flyouts;
using Twice.ViewModels.Twitter;
using Twice.Views.Services;

namespace Twice.ViewModels
{
	internal class Notifier : INotifier
	{
		public Notifier( IConfig config, IMessenger messenger, IDispatcher dispatcher, IViewServiceRepository viewServices )
		{
			Dispatcher = dispatcher;
			MessengerInstance = messenger;
			Config = config;
			ViewServices = viewServices;

			if( config.Notifications.SoundEnabled )
			{
				Player = new MediaPlayer();
				string soundFileName = File.Exists( config.Notifications.SoundFileName )
					? config.Notifications.SoundFileName
					: ResourceHelper.GetDefaultNotificationSound();

				Player.Open( new Uri( soundFileName ) );
			}

			if( config.Notifications.PopupEnabled )
			{
				PopupNotify = new NotifyIcon();
			}
		}

		public void PreviewInAppNotification( string message, bool top, int closeTime )
		{
			var context = new NotificationViewModel( message, NotificationType.Information, top )
			{
				CloseDelay = TimeSpan.FromSeconds( closeTime )
			};

			NotifyToast( context );
		}

		private void NotifyPopup( ColumnItem item, bool win10 )
		{
			if( win10 )
			{
				DisplayWin10Message( item.Text );
			}
		}

		private void NotifySound( ColumnItem item )
		{
			Dispatcher.CheckBeginInvokeOnUI( () => Player?.Play() );
		}

		private void NotifyToast( ColumnItem item )
		{
			var context = new NotificationViewModel( item, Config.Notifications.ToastsTop )
			{
				CloseDelay = TimeSpan.FromSeconds( Config.Notifications.ToastsCloseTime )
			};

			NotifyToast( context );
		}

		private void NotifyToast( NotificationViewModel vm )
		{
			vm.Dispatcher = Dispatcher;
			vm.MessengerInstance = MessengerInstance;

			ViewServices.OpenNotificationFlyout( vm );
		}

		public void DisplayMessage( string message, NotificationType type )
		{
			if( !Config.Notifications.ToastsEnabled )
			{
				return;
			}

			var context = new NotificationViewModel( message, type, Config.Notifications.ToastsTop )
			{
				CloseDelay = TimeSpan.FromSeconds( Config.Notifications.ToastsCloseTime )
			};

			NotifyToast( context );
		}

		public void DisplayWin10Message( string message )
		{
			var binding = new ToastBindingGeneric();
			binding.Children.Add( new AdaptiveText {Text = message, HintWrap = true} );

			ToastContent content = new ToastContent
			{
				Launch = "",
				Visual = new ToastVisual
				{
					BindingGeneric = binding
				}
			};

			var xml = content.GetContent();
			var doc = new XmlDocument();
			doc.LoadXml( xml );

			var n = ToastNotificationManager.CreateToastNotifier( Constants.ApplicationId );
			n.Show( new ToastNotification( doc ) );
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
				NotifyPopup( item, Config.Notifications.Win10Enabled );
			}
		}

		public void PreviewPopupNotification( string message, bool win10, string display, Corner displayCorner )
		{
			if( win10 )
			{
				DisplayWin10Message( message );
			}
		}

		private readonly IConfig Config;
		private readonly IDispatcher Dispatcher;
		private readonly IMessenger MessengerInstance;
		private readonly MediaPlayer Player;
		private readonly NotifyIcon PopupNotify;
		private readonly IViewServiceRepository ViewServices;
	}
}