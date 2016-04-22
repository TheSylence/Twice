using System.Collections.Generic;
using System.Linq;
using Twice.Models.Twitter;
using Twice.Utilities;
using Twice.ViewModels.Twitter;
using Twice.ViewModels.Wizards;

namespace Twice.ViewModels.ColumnManagement
{
	internal class UserSelectorPage : WizardPageViewModel
	{
		public UserSelectorPage( ITwitterContextList contextList, ITimerFactory timerFactory )
		{
			ContextList = contextList;
			Timer = timerFactory.Create( 1000 );
			Timer.Tick += Timer_Tick;

			UserCollection = new SmartCollection<UserViewModel>();
		}

		private async void Timer_Tick( object sender, System.EventArgs e )
		{
			IsLoading = true;
			Timer.Stop();

			// FIXME: Select the correct context
			var results = await ContextList.Contexts.First().Twitter.Users.Search( SearchText );
			UserCollection.Clear();
			UserCollection.AddRange( results.Select( u => new UserViewModel( u ) ) );

			IsLoading = false;
		}

		public bool IsLoading
		{
			[System.Diagnostics.DebuggerStepThrough]
			get
			{
				return _IsLoading;
			}
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

		public override int NextPageKey { get; protected set; } = -1;

		public string SearchText
		{
			[System.Diagnostics.DebuggerStepThrough]
			get
			{
				return _SearchText;
			}
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
		private readonly ITwitterContextList ContextList;
		private readonly ITimer Timer;
		private readonly SmartCollection<UserViewModel> UserCollection;

		[System.Diagnostics.DebuggerBrowsable( System.Diagnostics.DebuggerBrowsableState.Never )]
		private bool _IsLoading;

		[System.Diagnostics.DebuggerBrowsable( System.Diagnostics.DebuggerBrowsableState.Never )]
		private string _SearchText;
	}
}