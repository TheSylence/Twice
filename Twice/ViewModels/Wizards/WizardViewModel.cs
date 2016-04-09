using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;

namespace Twice.ViewModels.Wizards
{
	internal abstract class WizardViewModel : DialogViewModel
	{
		public TValue GetProperty<TValue>( string key )
		{
			return (TValue)Properties[key];
		}

		public void SetProperty( string key, object value )
		{
			Properties[key] = value;
		}

		protected virtual bool CanExecuteFinishCommand()
		{
			return true;
		}

		private bool CanExecuteGotoNextPageCommand()
		{
			return true;
		}

		private bool CanExecuteGotoPrevPageCommand()
		{
			return NavigationHistory.Count > 0;
		}

		protected virtual void ExecuteFinishCommand()
		{
			Close( true );
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

		public ICommand FinishCommand => _FinishCommand ?? ( _FinishCommand = new RelayCommand( ExecuteFinishCommand, CanExecuteFinishCommand ) );

		public ICommand GotoNextPageCommand => _GotoNextPageCommand ?? ( _GotoNextPageCommand = new RelayCommand( ExecuteGotoNextPageCommand, CanExecuteGotoNextPageCommand ) );

		public ICommand GotoPrevPageCommand => _GotoPrevPageCommand ?? ( _GotoPrevPageCommand = new RelayCommand( ExecuteGotoPrevPageCommand, CanExecuteGotoPrevPageCommand ) );

		protected readonly Dictionary<int, WizardPageViewModel> Pages = new Dictionary<int, WizardPageViewModel>();

		private readonly Stack<int> NavigationHistory = new Stack<int>();

		private readonly Dictionary<string, object> Properties = new Dictionary<string, object>();

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private WizardPageViewModel _CurrentPage;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private RelayCommand _FinishCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private RelayCommand _GotoNextPageCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private RelayCommand _GotoPrevPageCommand;

		private int CurrentPageKey = 0;
	}
}