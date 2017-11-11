using System;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using LinqToTwitter;
using Twice.Models.Proxy;
using Twice.Models.Twitter.Comparers;

namespace Twice.ViewModels.Twitter
{
	internal enum MediaType
	{
		Image,
		Animated
	}

	internal class StatusMediaViewModel : ObservableObject, IDragCanceller
	{
		public StatusMediaViewModel( Uri url, Uri displayUrl = null )
		{
			Url = MediaProxyServer.BuildUrl( url );
			DisplayUrl = displayUrl ?? url;
			Type = MediaType.Image;
		}

		public StatusMediaViewModel( MediaEntity entity, ulong userId = 0 )
		{
			Entity = entity;
			switch( entity.Type )
			{
			case "animated_gif":
			case "video":
				Url = MediaProxyServer.BuildUrl( entity.VideoInfo.Variants[0].Url, userId );
				DisplayUrl = new Uri( entity.VideoInfo.Variants[0].Url );
				Type = MediaType.Animated;
				break;

			default:
				Url = MediaProxyServer.BuildUrl( entity.MediaUrl, userId );
				DisplayUrl = new Uri( entity.ExpandedUrl );
				Type = MediaType.Image;
				break;
			}
		}

		public override bool Equals( object obj )
		{
			var other = obj as StatusMediaViewModel;
			if( other == null )
			{
				return false;
			}

			return TwitterComparers.MediaEntityComparer.Equals( Entity, other.Entity );
		}

		public override int GetHashCode()
		{
			return Entity?.GetHashCode() ?? 0;
		}

		private void ExecuteOpenImageCommand()
		{
			OpenRequested?.Invoke( this, EventArgs.Empty );
		}

		public Uri DisplayUrl { get; set; }

		public bool IsAnimated => Type == MediaType.Animated;

		public bool IsMuted { get; set; } = true;

		public bool IsPlaying { get; set; } = true;

		public bool Loop { get; set; } = true;

		public ICommand OpenImageCommand => _OpenImageCommand ?? ( _OpenImageCommand = new RelayCommand(
			                                    ExecuteOpenImageCommand ) );

		public Uri Url { get; }
		private readonly MediaEntity Entity;
		private readonly MediaType Type;
		private RelayCommand _OpenImageCommand;

		public event EventHandler OpenRequested;
	}
}