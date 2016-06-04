using System;
using System.Collections.Generic;

namespace Twice.ViewModels.Dialogs
{
	internal interface IImageDialogViewModel : IDialogViewModel
	{
		void SetImages( IEnumerable<Uri> urls );

		ICollection<ImageEntry> Images { get; }
		ImageEntry SelectedImage { get; set; }
	}
}