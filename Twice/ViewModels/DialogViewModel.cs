using System;
using System.Diagnostics;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using Twice.ViewModels.Validation;

namespace Twice.ViewModels
{
	internal interface IDialogViewModel
	{
		event EventHandler<CloseRequestEventArgs> CloseRequested;

		ICommand CancelCommand { get; }
		ICommand OkCommand { get; }
	}

	internal class CloseRequestEventArgs : EventArgs
	{
		public CloseRequestEventArgs( bool? result = null )
		{
			Result = result;
		}

		public static CloseRequestEventArgs Cancel { get; } = new CloseRequestEventArgs( false );
		public static CloseRequestEventArgs Ok { get; } = new CloseRequestEventArgs( true );
		public bool? Result { get; }
	}

	internal abstract class DialogViewModel : ValidationViewModel, IDialogViewModel
	{
		public event EventHandler<CloseRequestEventArgs> CloseRequested;

		protected virtual bool CanExecuteOkCommand()
		{
			return true;
		}

		protected virtual bool OnOk()
		{
			return true;
		}

		private void ExecuteCancelCommand()
		{
			CloseRequested?.Invoke( this, CloseRequestEventArgs.Cancel );
		}

		private void ExecuteOkCommand()
		{
			if( OnOk() )
			{
				CloseRequested?.Invoke( this, CloseRequestEventArgs.Ok );
			}
		}

		public ICommand CancelCommand => _CancelCommand ?? ( _CancelCommand = new RelayCommand( ExecuteCancelCommand ) );

		public ICommand OkCommand => _OkCommand ?? ( _OkCommand = new RelayCommand( ExecuteOkCommand, CanExecuteOkCommand ) );

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private RelayCommand _CancelCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private RelayCommand _OkCommand;
	}
}