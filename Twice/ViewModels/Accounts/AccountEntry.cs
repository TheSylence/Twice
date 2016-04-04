using System;
using System.Diagnostics;
using GalaSoft.MvvmLight;
using Twice.Models.Twitter;

namespace Twice.ViewModels.Accounts
{
	internal class AccountEntry : ObservableObject
	{
		public AccountEntry( IContextEntry context )
		{
			AccountName = context.AccountName;
			ProfileImage = context.ProfileImageUrl;
			IsDefaultAccount = context.IsDefault;
			RequiresConfirmation = context.RequiresConfirmation;
		}

		public event EventHandler ConfirmationChanged;

		public string AccountName { get; }

		public bool IsDefaultAccount { get; set; }

		public Uri ProfileImage { get; }

		public bool RequiresConfirmation
		{
			[DebuggerStepThrough] get { return _RequiresConfirmation; }
			set
			{
				if( _RequiresConfirmation == value )
				{
					return;
				}

				_RequiresConfirmation = value;
				RaisePropertyChanged();
				ConfirmationChanged?.Invoke( this, EventArgs.Empty );
			}
		}

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private bool _RequiresConfirmation;
	}
}