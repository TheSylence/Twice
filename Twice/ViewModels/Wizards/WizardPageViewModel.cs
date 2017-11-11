using System.Diagnostics;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace Twice.ViewModels.Wizards
{
	internal abstract class WizardPageViewModel : ObservableObject
	{
		protected WizardPageViewModel( IWizardViewModel wizard )
		{
			Wizard = wizard;
		}

		public virtual void OnNavigatedFrom( bool backward )
		{
		}

		public virtual void OnNavigatedTo( bool forward )
		{
		}

		protected virtual bool CanExecuteGotoNextPageCommand( object args )
		{
			return true;
		}

		protected virtual void ExecuteGotoNextPageCommand( object args )
		{
		}

		public ICommand GotoNextPageCommand
			=>
				_GotoNextPageCommand
				?? ( _GotoNextPageCommand = new RelayCommand<object>( ExecuteGotoNextPageCommand, CanExecuteGotoNextPageCommand ) );

		public bool IsLastPage { get; protected set; }
		protected readonly IWizardViewModel Wizard;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private RelayCommand<object> _GotoNextPageCommand;
	}
}