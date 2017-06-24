using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LinqToTwitter;
using Twice.Models.Columns;
using Twice.Models.Configuration;
using Twice.Models.Twitter;
using Twice.Models.Twitter.Streaming;

namespace Twice.ViewModels.Columns
{
	internal class UserColumn : ColumnViewModelBase
	{
		public UserColumn( IContextEntry context, ColumnDefinition definition, IConfig config, IStreamParser parser )
			: base( context, definition, config, parser )
		{
			UserId = definition.TargetAccounts.First();
			SubTitle = "";
		}

		protected override bool IsSuitableForColumn( Status status )
		{
			return status.GetUserId() == UserId;
		}

		protected override bool IsSuitableForColumn( DirectMessage message )
		{
			return false;
		}

		protected override async Task OnLoad( AsyncLoadContext context )
		{
			var userInfo = await Context.Twitter.Users.ShowUser( UserId, false );
			Title = userInfo.ScreenNameResponse;

			await base.OnLoad( context );
		}

		public override Icon Icon => Icon.User;

		protected override Expression<Func<Status, bool>> StatusFilterExpression => s => s.Type == StatusType.User && s.UserID == UserId;

		private readonly ulong UserId;
	}
}