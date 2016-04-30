using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;

namespace Twice.ViewModels.Wizards
{
	internal abstract class WizardViewModel : DialogViewModel, IWizardViewModel
	{
		protected virtual bool CanExecuteFinishCommand()
		{
			return true;
		}

		protected virtual void ExecuteFinishCommand()
		{
			Close( true );
		}

		private bool CanExecuteGotoPrevPageCommand()
		{
			return NavigationHistory.Count > 0;
		}

		private void ExecuteGotoPrevPageCommand()
		{
			CurrentPage?.OnNavigatedFrom( true );

			var key = NavigationHistory.Pop();
			CurrentPageKey = key;
			CurrentPage = Pages[key];

			CurrentPage?.OnNavigatedTo( false );
		}

		public TValue GetProperty<TValue>( string key )
		{
			return (TValue)Properties[key];
		}

		public void GotoPage( int key )
		{
			CurrentPage?.OnNavigatedFrom( false );

			WizardPageViewModel nextPage;
			if( Pages.TryGetValue( key, out nextPage ) )
			{
				NavigationHistory.Push( CurrentPageKey );
				CurrentPage = nextPage;
				CurrentPageKey = key;

				CurrentPage?.OnNavigatedTo( true );
			}

			// TODO: Handle this
		}

		public void SetProperty( string key, object value )
		{
			Properties[key] = value;
		}

		public WizardPageViewModel CurrentPage
		{
			[DebuggerStepThrough] get { return _CurrentPage; }
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

		public ICommand FinishCommand
			=> _FinishCommand ?? ( _FinishCommand = new RelayCommand( ExecuteFinishCommand, CanExecuteFinishCommand ) );

		public ICommand GotoPrevPageCommand
			=>
				_GotoPrevPageCommand
				?? ( _GotoPrevPageCommand = new RelayCommand( ExecuteGotoPrevPageCommand, CanExecuteGotoPrevPageCommand ) );

		private readonly Stack<int> NavigationHistory = new Stack<int>();

		protected readonly Dictionary<int, WizardPageViewModel> Pages = new Dictionary<int, WizardPageViewModel>();
		private readonly Dictionary<string, object> Properties = new Dictionary<string, object>();

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private WizardPageViewModel _CurrentPage;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private RelayCommand _FinishCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private RelayCommand _GotoPrevPageCommand;

		private int CurrentPageKey = 0;
	}
}