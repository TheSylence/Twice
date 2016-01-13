using System;
using System.Diagnostics;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;

namespace Twice.ViewModels
{
	internal interface IDialogViewModel
	{
		ICommand CancelCommand { get; }
		ICommand OkCommand { get; }
		event EventHandler<CloseRequestEventArgs> CloseRequested;
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
	internal abstract class DialogViewModel : IDialogViewModel
	{
		protected virtual bool CanExecuteOkCommand()
		{
			return true;
		}
		public event EventHandler<CloseRequestEventArgs> CloseRequested;

		protected virtual bool OnOk()
		{
			return true;
		}

		private void ExecuteCancelCommand()
		{
			if( OnOk() )
			{
				CloseRequested?.Invoke( this, CloseRequestEventArgs.Cancel );
			}
		}

		private void ExecuteOkCommand()
		{
			CloseRequested?.Invoke( this, CloseRequestEventArgs.Ok );
		}

		public ICommand CancelCommand => _CancelCommand ?? ( _CancelCommand = new RelayCommand( ExecuteCancelCommand ) );

		public ICommand OkCommand => _OkCommand ?? ( _OkCommand = new RelayCommand( ExecuteOkCommand, CanExecuteOkCommand ) );

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private RelayCommand _CancelCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private RelayCommand _OkCommand;
	}
}