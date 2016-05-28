using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Diagnostics;
using System.Windows.Input;
using Twice.Resources;
using Twice.ViewModels.Validation;

namespace Twice.ViewModels.Settings
{
	internal class MuteEditViewModel : ValidationViewModel, IMuteEditViewModel
	{
		public MuteEditViewModel( MuteEditAction action )
		{
			Action = action;

			Validate( () => Filter ).NotEmpty();
			Validate( () => EndDate )
				.If( () => HasEndDate )
				.Check( dt => DateTime.Now < dt )
				.Message( Strings.DastMustNotBeInThePast );

			EndDate = DateTime.Now.AddMonths( 1 );
			HasEndDate = false;
			CaseSensitive = false;
		}

		public event EventHandler Cancelled;

		public event EventHandler<MuteEditArgs> Saved;

		private bool CanExecuteSaveCommand()
		{
			return !HasErrors;
		}

		private void ExecuteCancelCommand()
		{
			Cancelled?.Invoke( this, EventArgs.Empty );
		}

		private void ExecuteSaveCommand()
		{
			ValidateAll();
			if( HasErrors )
			{
				return;
			}

			DateTime? dt = null;
			if( HasEndDate )
			{
				dt = EndDate;
			}

			Saved?.Invoke( this, new MuteEditArgs( Action, Filter, dt ) );
		}

		public ICommand CancelCommand => _CancelCommand ?? ( _CancelCommand = new RelayCommand( ExecuteCancelCommand ) );

		public bool CaseSensitive
		{
			[DebuggerStepThrough]
			get { return _CaseSensitive; }
			set
			{
				if( _CaseSensitive == value )
				{
					return;
				}

				_CaseSensitive = value;
				RaisePropertyChanged();
			}
		}

		public DateTime EndDate
		{
			[DebuggerStepThrough]
			get { return _EndDate; }
			set
			{
				if( _EndDate == value )
				{
					return;
				}

				_EndDate = value;
				RaisePropertyChanged();
			}
		}

		public string Filter
		{
			[DebuggerStepThrough]
			get { return _Filter; }
			set
			{
				if( _Filter == value )
				{
					return;
				}

				_Filter = value;
				RaisePropertyChanged();
			}
		}

		public bool HasEndDate
		{
			[DebuggerStepThrough]
			get { return _HasEndDate; }
			set
			{
				if( _HasEndDate == value )
				{
					return;
				}

				_HasEndDate = value;
				RaisePropertyChanged();
				RaiseErrorsChanged( nameof( EndDate ) );
				RaisePropertyChanged( nameof( EndDate ) );
			}
		}

		public ICommand SaveCommand
			=> _SaveCommand ?? ( _SaveCommand = new RelayCommand( ExecuteSaveCommand, CanExecuteSaveCommand ) );

		private readonly MuteEditAction Action;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private RelayCommand _CancelCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private bool _CaseSensitive;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private DateTime _EndDate;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private string _Filter;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private bool _HasEndDate;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private RelayCommand _SaveCommand;
	}
}