using GalaSoft.MvvmLight.CommandWpf;
using MaterialDesignThemes.Wpf;
using Ninject;
using System;
using System.Diagnostics;
using System.Windows.Input;
using Twice.Utilities;
using Twice.Utilities.Ui;
using Twice.ViewModels.Validation;

namespace Twice.ViewModels
{
	internal class DialogViewModel : ValidationViewModel, IDialogViewModel
	{
		public event EventHandler<CloseEventArgs> CloseRequested;

		protected virtual bool CanExecuteOkCommand()
		{
			return true;
		}

		protected void Close( bool result )
		{
			Dispatcher.CheckBeginInvokeOnUI( () =>
			{
				DialogHost.CloseDialogCommand.Execute( result, ViewServiceRepository?.CurrentDialog );
				CloseRequested?.Invoke( this, result ? CloseEventArgs.Ok : CloseEventArgs.Cancel );
			} );
		}

		protected virtual bool OnOk()
		{
			return !HasErrors;
		}

		private void ExecuteCancelCommand()
		{
			Close( false );
		}

		private void ExecuteOkCommand()
		{
			ValidateAll();
			if( OnOk() )
			{
				Close( true );
			}
		}

		public ICommand CancelCommand => _CancelCommand ?? ( _CancelCommand = new RelayCommand( ExecuteCancelCommand ) );

		[Inject]
		public IDispatcher Dispatcher { get; set; }

		public ICommand OkCommand => _OkCommand ?? ( _OkCommand = new RelayCommand( ExecuteOkCommand, CanExecuteOkCommand ) );

		public string Title
		{
			[DebuggerStepThrough]
			get { return _Title; }
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

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private RelayCommand _CancelCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private RelayCommand _OkCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private string _Title;
	}
}