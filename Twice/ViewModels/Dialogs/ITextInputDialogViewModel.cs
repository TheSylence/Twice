namespace Twice.ViewModels.Dialogs
{
	internal interface ITextInputDialogViewModel : IDialogViewModel
	{
		string Input { get; set; }
		string Label { get; set; }
	}
}