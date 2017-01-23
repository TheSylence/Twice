using System;
using System.Collections.Generic;
using System.Windows.Input;
using Twice.ViewModels.Twitter;

namespace Twice.ViewModels.Dialogs
{
	internal interface IImageDialogViewModel : IDialogViewModel
	{
		void SetImages( IEnumerable<StatusMediaViewModel> images );

		void SetImages( IEnumerable<Uri> images );

		ICommand CopyToClipboardCommand { get; }
		ICollection<ImageEntry> Images { get; }
		ICommand OpenImageCommand { get; }
		ImageEntry SelectedImage { get; set; }
	}
}