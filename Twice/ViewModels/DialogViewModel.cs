using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using Ninject;
using Twice.Models.Cache;
using Twice.Utilities.Ui;
using Twice.ViewModels.Validation;

namespace Twice.ViewModels
{
	internal class DialogViewModel : ValidationViewModel, IDialogViewModel
	{
		protected virtual bool CanExecuteOkCommand()
		{
			return !HasErrors;
		}

		protected void Close( bool result )
		{
			Dispatcher?.CheckBeginInvokeOnUI( () =>
			{
				CloseRequested?.Invoke( this, result
					? CloseEventArgs.Ok
					: CloseEventArgs.Cancel );
			} );
		}

		protected virtual Task<bool> OnOk()
		{
			return Task.FromResult( !HasErrors );
		}

		private void ExecuteCancelCommand()
		{
			Close( false );
		}

		private async void ExecuteOkCommand()
		{
			ValidateAll();
			if( await OnOk() )
			{
				Close( true );
			}
		}

		public ICommand CancelCommand => _CancelCommand ?? ( _CancelCommand = new RelayCommand( ExecuteCancelCommand ) );

		public ICommand OkCommand => _OkCommand ?? ( _OkCommand = new RelayCommand( ExecuteOkCommand, CanExecuteOkCommand ) );

		public string Title { get; set; }

		public void Center()
		{
			CenterRequested?.Invoke( this, EventArgs.Empty );
		}

		public event EventHandler CenterRequested;

		public event EventHandler<CloseEventArgs> CloseRequested;

		[Inject]
		public ICache Cache { protected get; set; }

		[Inject]
		public IDispatcher Dispatcher { protected get; set; }

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private RelayCommand _CancelCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private RelayCommand _OkCommand;
	}
}