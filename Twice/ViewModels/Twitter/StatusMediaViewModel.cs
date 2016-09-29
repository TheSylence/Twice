using System;
using System.Diagnostics;
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
			DisplayUrl = url;
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
				DisplayUrl = new Uri( entity.VideoInfo.Variants[0].Url );
				Type = MediaType.Animated;
				break;

			default:
				Url = new Uri( entity.MediaUrl );
				DisplayUrl = Url;
				Type = MediaType.Image;
				break;
			}
		}

		public event EventHandler OpenRequested;

		private void ExecuteOpenImageCommand()
		{
			OpenRequested?.Invoke( this, EventArgs.Empty );
		}

		public Uri DisplayUrl
		{
			[DebuggerStepThrough] get { return _DisplayUrl; }
			set
			{
				if( _DisplayUrl == value )
				{
					return;
				}

				_DisplayUrl = value;
				RaisePropertyChanged( nameof( DisplayUrl ) );
			}
		}

		public bool IsAnimated => Type == MediaType.Animated;

		public bool IsMuted
		{
			[DebuggerStepThrough] get { return _IsMuted; }
			set
			{
				if( _IsMuted == value )
				{
					return;
				}

				_IsMuted = value;
				RaisePropertyChanged();
			}
		}

		public bool IsPlaying
		{
			[DebuggerStepThrough] get { return _IsPlaying; }
			set
			{
				if( _IsPlaying == value )
				{
					return;
				}

				_IsPlaying = value;
				RaisePropertyChanged();
			}
		}

		public bool Loop
		{
			[DebuggerStepThrough] get { return _Loop; }
			set
			{
				if( _Loop == value )
				{
					return;
				}

				_Loop = value;
				RaisePropertyChanged();
			}
		}

		public ICommand OpenImageCommand => _OpenImageCommand ?? ( _OpenImageCommand = new RelayCommand(
			                                    ExecuteOpenImageCommand ) );

		public Uri Url { get; }

		private readonly MediaType Type;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private Uri _DisplayUrl;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private bool _IsMuted = true;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private bool _IsPlaying = true;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private bool _Loop = true;

		private RelayCommand _OpenImageCommand;
		private MediaEntity Entity;
	}
}