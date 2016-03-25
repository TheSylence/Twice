using System;
using System.IO;
using System.Windows.Forms;
using System.Windows.Media;
using Twice.Models.Configuration;
using Twice.ViewModels.Columns.Definitions;
using Twice.ViewModels.Twitter;
using Windows.UI.Notifications;

namespace Twice.ViewModels
{
	internal interface INotifier
	{
		void OnStatus( StatusViewModel status, ColumnNotifications columnSettings );
	}

	internal class Notifier : INotifier
	{
		public Notifier( IConfig config )
		{
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
		}

		private readonly IConfig Config;
		private readonly MediaPlayer Player;
		private readonly NotifyIcon PopupNotify;
	}
}