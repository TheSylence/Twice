using System;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using LinqToTwitter;

namespace Twice.ViewModels.Twitter
{
	internal enum MediaType
	{
		Image,
		Animated
	}

	internal class StatusMediaViewModel : ObservableObject
	{
		public StatusMediaViewModel( Uri url )
		{
			Url = GetUrl( url );
			Type = MediaType.Image;
		}

		public StatusMediaViewModel( MediaEntity entity )
		{
			Entity = entity;
			switch( entity.Type )
			{
			case "animated_gif":
			case "video":
				Url = GetUrl( entity.VideoInfo.Variants[0].Url );
				Type = MediaType.Animated;
				break;

			default:
				Url = new Uri( entity.MediaUrl );
				Type = MediaType.Image;
				break;
			}
		}

		public event EventHandler OpenRequested;

		private static Uri GetUrl( Uri mediaUrl )
		{
			const string baseUrl = "http://localhost:60123/twice/media/?stream=";

			return new Uri( baseUrl + Uri.EscapeUriString( mediaUrl.AbsoluteUri ) );
		}

		private static Uri GetUrl( string mediaUrl )
		{
			return GetUrl( new Uri( mediaUrl ) );
		}

		private void ExecuteOpenImageCommand()
		{
			OpenRequested?.Invoke( this, EventArgs.Empty );
		}

		public ICommand OpenImageCommand => _OpenImageCommand ?? ( _OpenImageCommand = new RelayCommand(
			ExecuteOpenImageCommand ) );

		public MediaType Type { get; }
		public Uri Url { get; }

		private RelayCommand _OpenImageCommand;
		private MediaEntity Entity;
	}
}