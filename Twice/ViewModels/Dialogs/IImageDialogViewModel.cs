using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Twice.ViewModels.Dialogs
{
	internal interface IImageDialogViewModel : IDialogViewModel
	{
		void SetImages( IEnumerable<Uri> urls );

		ICommand CopyToClipboardCommand { get; }
		ICollection<ImageEntry> Images { get; }
		ICommand OpenImageCommand { get; }
		ImageEntry SelectedImage { get; set; }
	}
}