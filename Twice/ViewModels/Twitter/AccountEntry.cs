using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using JetBrains.Annotations;
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

		[UsedImplicitly]
		private void OnUseChanged()
		{
			UseChanged?.Invoke( this, EventArgs.Empty );
		}

		public IContextEntry Context { get; }
		public ImageSource Image { get; }
		public bool IsDefault => Context.IsDefault;
		public string ScreenName => Context.AccountName;

		public bool Use { get; set; }

		public event EventHandler UseChanged;
	}
}