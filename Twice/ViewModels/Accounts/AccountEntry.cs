using GalaSoft.MvvmLight;
using System;
using System.Diagnostics;
using Twice.Models.Twitter;

namespace Twice.ViewModels.Accounts
{
	internal class AccountEntry : ObservableObject
	{
		public AccountEntry( IContextEntry context )
		{
			Context = context;
			AccountName = Context.AccountName;
			ProfileImage = Context.ProfileImageUrl;
			_RequiresConfirmation = Context.RequiresConfirmation;
		}

		public event EventHandler ConfirmationChanged;

		public string AccountName { get; }
		public TwitterAccountData Data => Context.Data;

		public bool IsDefaultAccount
		{
			get => Context.IsDefault;
			set
			{
				Context.IsDefault = value;
				RaisePropertyChanged();
			}
		}

		public Uri ProfileImage { get; }

		public bool RequiresConfirmation
		{
			[DebuggerStepThrough]
			get { return _RequiresConfirmation; }
			set
			{
				if( _RequiresConfirmation == value )
				{
					return;
				}

				_RequiresConfirmation = value;
				RaisePropertyChanged();
				Data.RequiresConfirm = value;
				ConfirmationChanged?.Invoke( this, EventArgs.Empty );
			}
		}

		private readonly IContextEntry Context;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private bool _RequiresConfirmation;
	}
}