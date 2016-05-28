using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Twice.Models.Twitter;
using Twice.Utilities;
using Twice.ViewModels.Twitter;
using Twice.ViewModels.Wizards;

namespace Twice.ViewModels.ColumnManagement
{
	internal class UserSelectorPage : WizardPageViewModel
	{
		public UserSelectorPage( IWizardViewModel wizard, ITimerFactory timerFactory )
			: base( wizard )
		{
			Timer = timerFactory.Create( 1000 );
			Timer.Tick += Timer_Tick;

			UserCollection = new SmartCollection<UserViewModel>();
		}

		protected override void ExecuteGotoNextPageCommand( object args )
		{
			ulong userId = (ulong)args;
			var targetAccounts = Wizard.GetProperty<ulong[]>( AddColumnDialogViewModel.TargetAccountsKey ).ToList();
			targetAccounts.Add( userId );
			int pageKey = 3;
			Wizard.SetProperty( AddColumnDialogViewModel.TargetAccountsKey, targetAccounts.ToArray() );

			var screenNames = Wizard.GetProperty<string[]>( AddColumnDialogViewModel.TargetAccountNamesKey ).ToList();
			var screenName = Users.Single( usr => usr.UserId == userId ).Model.GetScreenName();
			screenNames.Add( screenName );
			Wizard.SetProperty( AddColumnDialogViewModel.TargetAccountNamesKey, screenNames.ToArray() );
			Wizard.GotoPage( pageKey );
		}

		private async void Timer_Tick( object sender, EventArgs e )
		{
			IsLoading = true;
			Timer.Stop();

			var contexts = Wizard.GetProperty<IContextEntry[]>( AddColumnDialogViewModel.ContextsKey );

			var results = await contexts.First().Twitter.Users.Search( SearchText );
			UserCollection.Clear();
			UserCollection.AddRange( results.Select( u => new UserViewModel( u ) ) );

			IsLoading = false;
		}

		public bool IsLoading
		{
			[DebuggerStepThrough] get { return _IsLoading; }
			set
			{
				if( _IsLoading == value )
				{
					return;
				}

				_IsLoading = value;
				RaisePropertyChanged();
			}
		}

		public string SearchText
		{
			[DebuggerStepThrough] get { return _SearchText; }
			set
			{
				if( _SearchText == value )
				{
					return;
				}

				Timer.Stop();
				Timer.Start();

				_SearchText = value;
				RaisePropertyChanged();
			}
		}

		public ICollection<UserViewModel> Users => UserCollection;
		private readonly ITimer Timer;
		private readonly SmartCollection<UserViewModel> UserCollection;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private bool _IsLoading;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private string _SearchText;
	}
}