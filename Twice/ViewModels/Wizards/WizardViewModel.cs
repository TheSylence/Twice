using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;

namespace Twice.ViewModels.Wizards
{
	internal abstract class WizardViewModel : DialogViewModel
	{
		private bool CanExecuteGotoNextPageCommand()
		{
			return true;
		}

		private bool CanExecuteGotoPrevPageCommand()
		{
			return true;
		}

		private void ExecuteGotoNextPageCommand()
		{
		}

		private void ExecuteGotoPrevPageCommand()
		{
		}

		public WizardPageViewModel CurrentPage
		{
			[DebuggerStepThrough]
			get
			{
				return _CurrentPage;
			}
			set
			{
				if( _CurrentPage == value )
				{
					return;
				}

				_CurrentPage = value;
				RaisePropertyChanged();
			}
		}

		public ICommand GotoNextPageCommand => _GotoNextPageCommand ?? ( _GotoNextPageCommand = new RelayCommand( ExecuteGotoNextPageCommand, CanExecuteGotoNextPageCommand ) );

		public ICommand GotoPrevPageCommand => _GotoPrevPageCommand ?? ( _GotoPrevPageCommand = new RelayCommand( ExecuteGotoPrevPageCommand, CanExecuteGotoPrevPageCommand ) );

		protected readonly Dictionary<int, WizardPageViewModel> Pages = new Dictionary<int, WizardPageViewModel>();

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private WizardPageViewModel _CurrentPage;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private RelayCommand _GotoNextPageCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private RelayCommand _GotoPrevPageCommand;
	}
}