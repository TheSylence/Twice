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
			return NavigationHistory.Count > 0;
		}

		private void ExecuteGotoNextPageCommand()
		{
			WizardPageViewModel nextPage;
			if( Pages.TryGetValue( CurrentPage.NextPageKey, out nextPage ) )
			{
				NavigationHistory.Push( CurrentPageKey );
				CurrentPage = nextPage;
				CurrentPageKey = CurrentPage.NextPageKey;
			}

			// TODO: Handle this
		}

		private void ExecuteGotoPrevPageCommand()
		{
			var key = NavigationHistory.Pop();
			CurrentPageKey = key;
			CurrentPage = Pages[key];
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
		private readonly Stack<int> NavigationHistory = new Stack<int>();

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private WizardPageViewModel _CurrentPage;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private RelayCommand _GotoNextPageCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private RelayCommand _GotoPrevPageCommand;

		private int CurrentPageKey = 0;
	}
}