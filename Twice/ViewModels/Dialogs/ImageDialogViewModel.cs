using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Twice.ViewModels.Dialogs
{
	internal class ImageDialogViewModel : DialogViewModel, IImageDialogViewModel
	{
		public ImageDialogViewModel()
		{
			Images = new ObservableCollection<ImageEntry>();
		}

		public void SetImages( IEnumerable<Uri> urls )
		{
			Images.Clear();
			foreach( var url in urls )
			{
				Images.Add( new ImageEntry( url ) );
			}
		}

		public ICollection<ImageEntry> Images { get; }

		public ImageEntry SelectedImage
		{
			[DebuggerStepThrough]
			get { return _SelectedImage; }
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

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private ImageEntry _SelectedImage;
	}
}