using System;
using System.Diagnostics;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
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

		public event EventHandler Cancelled;

		public bool CaseSensitive { get; set; }

		public DateTime EndDate { get; set; }

		public string Filter { get; set; }

		public bool HasEndDate
		{
			[DebuggerStepThrough] get { return _HasEndDate; }
			set
			{
				if( _HasEndDate == value )
				{
					return;
				}

				_HasEndDate = value;
				RaisePropertyChanged();
				RaiseErrorsChanged( nameof(EndDate) );
				RaisePropertyChanged( nameof(EndDate) );
			}
		}

		public ICommand SaveCommand
			=> _SaveCommand ?? ( _SaveCommand = new RelayCommand( ExecuteSaveCommand, CanExecuteSaveCommand ) );

		public event EventHandler<MuteEditArgs> Saved;

		private readonly MuteEditAction Action;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private RelayCommand _CancelCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private bool _HasEndDate;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private RelayCommand _SaveCommand;
	}
}