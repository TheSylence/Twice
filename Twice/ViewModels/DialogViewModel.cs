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
		public event EventHandler<CloseEventArgs> CloseRequested;

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

		[Inject]
		public ICache Cache { get; set; }

		public ICommand CancelCommand => _CancelCommand ?? ( _CancelCommand = new RelayCommand( ExecuteCancelCommand ) );

		[Inject]
		public IDispatcher Dispatcher { get; set; }

		public ICommand OkCommand => _OkCommand ?? ( _OkCommand = new RelayCommand( ExecuteOkCommand, CanExecuteOkCommand ) );

		public string Title
		{
			[DebuggerStepThrough] get { return _Title; }
			set
			{
				if( _Title == value )
				{
					return;
				}

				_Title = value;
				RaisePropertyChanged();
			}
		}

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private RelayCommand _CancelCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private RelayCommand _OkCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private string _Title;
	}
}