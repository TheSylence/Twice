using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;
using Twice.Models.Twitter;
using Twice.ViewModels.Twitter;

namespace Twice.ViewModels.Columns
{
	internal class UserColumn : ColumnViewModelBase
	{
		public UserColumn( IContextEntry context, ulong userId )
			: base( context )
		{
			UserId = userId;
			Statuses = new ObservableCollection<StatusViewModel>();
		}

		protected override async Task OnLoad()
		{
			var userInfo = await Context.Twitter.User.Where( u => u.UserID == UserId && u.Type == UserType.Show ).FirstAsync( );
			Title = userInfo.ScreenNameResponse;
			RaisePropertyChanged( nameof( Title ) );

			var statuses = await Context.Twitter.Status.Where( s => s.Type == StatusType.User && s.UserID == UserId ).ToListAsync();

			foreach( var status in  statuses.Select( t => new StatusViewModel( t, Context ) ) )
			{
				Statuses.Add( status );
			}
		}

		private readonly ulong UserId;

		public override Icon Icon => Icon.User;

		public override ICollection<StatusViewModel> Statuses { get; }
		public override string Title { get; protected set; }
	}
}