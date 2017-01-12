using System.Diagnostics;

namespace Twice.ViewModels.Dialogs
{
	internal class ConfirmDialogViewModel : DialogViewModel, IConfirmDialogViewModel
	{
		public string Label
		{
			[DebuggerStepThrough]
			get { return _Label; }
			set
			{
				if( _Label == value )
				{
					return;
				}

				_Label = value;
				RaisePropertyChanged();
			}
		}

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private string _Label;
	}
}