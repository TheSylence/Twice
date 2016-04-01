using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using Twice.Models.Twitter;

namespace Twice.ViewModels.Twitter
{
	internal class AccountEntry : ObservableObject
	{
		public AccountEntry( IContextEntry context )
		{
			Context = context;
			Image = new BitmapImage( context.ProfileImageUrl );
		}

		public IContextEntry Context { get; }
		public ImageSource Image { get; }
		public string ScreenName => Context.AccountName;

		public bool Use
		{
			[DebuggerStepThrough]
			get
			{
				return _Use;
			}
			set
			{
				if( _Use == value )
				{
					return;
				}

				_Use = value;
				RaisePropertyChanged();
			}
		}

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private bool _Use;
	}
}