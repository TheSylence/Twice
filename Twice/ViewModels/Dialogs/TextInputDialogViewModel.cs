using System.Diagnostics;
using Twice.ViewModels.Validation;

namespace Twice.ViewModels.Dialogs
{
	internal class TextInputDialogViewModel : DialogViewModel, ITextInputDialogViewModel
	{
		public TextInputDialogViewModel()
		{
			Validate( () => Input ).NotEmpty();
		}

		public string Input
		{
			[DebuggerStepThrough]
			get { return _Input; }
			set
			{
				if( _Input == value )
				{
					return;
				}

				_Input = value;
				RaisePropertyChanged();
			}
		}

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

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private string _Input;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private string _Label;
	}
}