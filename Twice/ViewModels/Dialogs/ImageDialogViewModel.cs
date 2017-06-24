using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using Ninject;
using Twice.Models.Proxy;
using Twice.Resources;
using Twice.Utilities.Os;
using Twice.ViewModels.Twitter;

namespace Twice.ViewModels.Dialogs
{
	internal class ImageDialogViewModel : DialogViewModel, IImageDialogViewModel
	{
		public ImageDialogViewModel()
		{
			Images = new ObservableCollection<ImageEntry>();
			Title = Strings.ImageViewer;
		}

		private void ExecuteCopyToClipboardCommand()
		{
			Clipboard.SetText( SelectedImage.DisplayUrl.AbsoluteUri );
		}

		private void ExecuteOpenImageCommand()
		{
			ProcessStarter.Start( SelectedImage.DisplayUrl.AbsoluteUri );
		}

		public ICommand CopyToClipboardCommand
			=> _CopyToClipboardCommand ?? ( _CopyToClipboardCommand = new RelayCommand( ExecuteCopyToClipboardCommand ) );

		public ICollection<ImageEntry> Images { get; }

		public ICommand OpenImageCommand
			=> _OpenImageCommand ?? ( _OpenImageCommand = new RelayCommand( ExecuteOpenImageCommand ) );

		public ImageEntry SelectedImage
		{
			[DebuggerStepThrough] get { return _SelectedImage; }
			set
			{
				if( _SelectedImage == value )
				{
					return;
				}

				_SelectedImage = value;
				RaisePropertyChanged();
			}
		}

		public void SetImages( IEnumerable<Uri> images )
		{
			Images.Clear();
			foreach( var url in images )
			{
				var displayUrl = MediaProxyServer.ExtractUrl( url );

				Images.Add( new ImageEntry( url, false, displayUrl.AbsoluteUri, displayUrl ) );
			}

			Center();
		}

		public void SetImages( IEnumerable<StatusMediaViewModel> images )
		{
			Images.Clear();
			foreach( var url in images )
			{
				Images.Add( new ImageEntry( url ) );
			}

			Center();
		}

		[Inject]
		public IClipboard Clipboard { get; set; }

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private RelayCommand _CopyToClipboardCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private RelayCommand _OpenImageCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private ImageEntry _SelectedImage;
	}
}