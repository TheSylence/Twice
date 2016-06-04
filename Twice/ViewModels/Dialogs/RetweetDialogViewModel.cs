using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using Twice.ViewModels.Twitter;

namespace Twice.ViewModels.Dialogs
{
	internal class RetweetDialogViewModel : DialogViewModel, IRetweetDialogViewModel
	{
		public RetweetDialogViewModel()
		{
			Accounts = new List<AccountEntry>();
		}

		public Task OnLoad( object data )
		{
			foreach( var acc in Accounts )
			{
				acc.UseChanged -= Acc_UseChanged;
			}

			Accounts = ContextList.Contexts.Select( c => new AccountEntry( c ) ).ToList();
			foreach( var acc in Accounts )
			{
				acc.UseChanged += Acc_UseChanged;
			}

			var defAccount = Accounts.FirstOrDefault( a => a.IsDefault ) ?? Accounts.First();
			defAccount.Use = true;
			RaisePropertyChanged( nameof( Accounts ) );

			if( Accounts.Count == 1 )
			{
				ExecuteRetweetCommand();
				Close( true );
			}

			return Task.CompletedTask;
		}

		private void Acc_UseChanged( object sender, EventArgs e )
		{
			RaisePropertyChanged( nameof( ConfirmationRequired ) );
		}

		private bool CanExecuteQuoteCommand()
		{
			return Accounts.Any( a => a.Use );
		}

		private bool CanExecuteRetweetCommand()
		{
			return Accounts.Any( a => a.Use );
		}

		private async void ExecuteQuoteCommand()
		{
			await ViewServiceRepository.QuoteTweet( Status, Accounts.Where( a => a.Use ).Select( a => a.Context.UserId ) );
		}

		private void ExecuteRetweetCommand()
		{
			foreach( var acc in Accounts.Where( a => a.Use ) )
			{
				Status.RetweetStatus( acc.Context.Twitter );
			}
		}

		public ICollection<AccountEntry> Accounts { get; private set; }

		public bool ConfirmationRequired
		{
			get { return Accounts.Where( a => a.Use ).Any( a => a.Context.RequiresConfirmation ); }
		}

		public bool ConfirmationSet
		{
			[DebuggerStepThrough] get { return _ConfirmationSet; }
			set
			{
				if( _ConfirmationSet == value )
				{
					return;
				}

				_ConfirmationSet = value;
				RaisePropertyChanged();
			}
		}

		public ICommand QuoteCommand => _QuoteCommand ?? ( _QuoteCommand = new RelayCommand(
			ExecuteQuoteCommand, CanExecuteQuoteCommand ) );

		public ICommand RetweetCommand => _RetweetCommand ?? ( _RetweetCommand = new RelayCommand(
			ExecuteRetweetCommand, CanExecuteRetweetCommand ) );

		public StatusViewModel Status
		{
			[DebuggerStepThrough] get { return _Status; }
			set
			{
				if( _Status == value )
				{
					return;
				}

				_Status = value;
				RaisePropertyChanged();
			}
		}

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private bool _ConfirmationSet;

		private RelayCommand _QuoteCommand;
		private RelayCommand _RetweetCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private StatusViewModel _Status;
	}
}