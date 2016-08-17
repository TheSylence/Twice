using System;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using LinqToTwitter;
using Twice.Models.Proxy;

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
			Url = MediaProxyServer.BuildUrl( url );
			Type = MediaType.Image;
		}

		public StatusMediaViewModel( MediaEntity entity )
		{
			Entity = entity;
			switch( entity.Type )
			{
			case "animated_gif":
			case "video":
				Url = MediaProxyServer.BuildUrl( entity.VideoInfo.Variants[0].Url );
				Type = MediaType.Animated;
				break;

			default:
				Url = new Uri( entity.MediaUrl );
				Type = MediaType.Image;
				break;
			}
		}

		public event EventHandler OpenRequested;

		private void ExecuteOpenImageCommand()
		{
			OpenRequested?.Invoke( this, EventArgs.Empty );
		}

		public bool IsAnimated => Type == MediaType.Animated;

		public ICommand OpenImageCommand => _OpenImageCommand ?? ( _OpenImageCommand = new RelayCommand(
			ExecuteOpenImageCommand ) );

		public Uri Url { get; }
		private readonly MediaType Type;
		private RelayCommand _OpenImageCommand;
		private MediaEntity Entity;
	}
}