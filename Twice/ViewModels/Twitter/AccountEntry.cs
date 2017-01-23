using GalaSoft.MvvmLight;
using System;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Twice.Models.Twitter;

namespace Twice.ViewModels.Twitter
{
	internal class AccountEntry : ObservableObject
	{
		/// <summary>
		/// </summary>
		/// <param name="context"></param>
		/// <param name="loadImage"> Disable this during tests so wpf won't try to load the image </param>
		public AccountEntry( IContextEntry context, bool loadImage = true )
		{
			Context = context;
			Image = loadImage ? new BitmapImage( context.ProfileImageUrl ) : new BitmapImage();
		}

		public event EventHandler UseChanged;

		public IContextEntry Context { get; }
		public ImageSource Image { get; }
		public bool IsDefault => Context.IsDefault;
		public string ScreenName => Context.AccountName;

		public bool Use
		{
			[DebuggerStepThrough]
			get { return _Use; }
			set
			{
				if( _Use == value )
				{
					return;
				}

				_Use = value;
				RaisePropertyChanged();
				UseChanged?.Invoke( this, EventArgs.Empty );
			}
		}

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private bool _Use;
	}
}