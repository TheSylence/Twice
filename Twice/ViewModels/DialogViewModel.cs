using System;
using System.Diagnostics;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using MaterialDesignThemes.Wpf;
using Twice.ViewModels.Validation;

namespace Twice.ViewModels
{
	internal interface IDialogViewModel
	{
		ICommand CancelCommand { get; }
		ICommand OkCommand { get; }
		string Title { get; set; }
	}

	internal abstract class DialogViewModel : ValidationViewModel, IDialogViewModel
	{
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
			DialogHost.CloseDialogCommand.Execute( false, ViewServiceRepository.CurrentDialog );
		}

		private void ExecuteOkCommand()
		{
			if( OnOk() )
			{
				DialogHost.CloseDialogCommand.Execute( true, ViewServiceRepository.CurrentDialog );
			}
		}

		public ICommand CancelCommand => _CancelCommand ?? ( _CancelCommand = new RelayCommand( ExecuteCancelCommand ) );

		public ICommand OkCommand => _OkCommand ?? ( _OkCommand = new RelayCommand( ExecuteOkCommand, CanExecuteOkCommand ) );

		public string Title
		{
			[DebuggerStepThrough]
			get
			{
				return _Title;
			}
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