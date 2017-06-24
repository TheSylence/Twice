using Twice.ViewModels.Validation;

namespace Twice.ViewModels.Dialogs
{
	internal class TextInputDialogViewModel : DialogViewModel, ITextInputDialogViewModel
	{
		public TextInputDialogViewModel()
		{
			Validate( () => Input ).NotEmpty();
		}

		public string Input { get; set; }

		public string Label { get; set; }
	}
}