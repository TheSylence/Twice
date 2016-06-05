using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using Ninject;
using Twice.Utilities.Os;

namespace Twice.ViewModels.Dialogs
{
	internal class ImageDialogViewModel : DialogViewModel, IImageDialogViewModel
	{
		public ImageDialogViewModel()
		{
			Images = new ObservableCollection<ImageEntry>();
		}

		private void ExecuteCopyToClipboardCommand()
		{
			Clipboard.SetText( SelectedImage.ImageUrl.AbsoluteUri );
		}

		private void ExecuteOpenImageCommand()
		{
			ProcessStarter.Start( SelectedImage.ImageUrl.AbsoluteUri );
		}

		public void SetImages( IEnumerable<Uri> urls )
		{
			Images.Clear();
			foreach( var url in urls )
			{
				Images.Add( new ImageEntry( url ) );
			}
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

		[Inject]
		public IClipboard Clipboard { get; set; }

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private RelayCommand _CopyToClipboardCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private RelayCommand _OpenImageCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private ImageEntry _SelectedImage;
	}
}