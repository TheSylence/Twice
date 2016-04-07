using LinqToTwitter;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
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
		}

		protected override bool IsSuitableForColumn( Status status )
		{
			if( status.UserID == UserId )
			{
				return true;
			}
			ulong id;
			if( ulong.TryParse( status.User.UserIDResponse, out id ) )
			{
				return id == UserId;
			}

			return status.User.UserID == UserId;
		}

		protected override async Task OnLoad()
		{
			var userInfo = await Context.Twitter.User.Where( u => u.UserID == UserId && u.Type == UserType.Show ).FirstAsync();
			Title = userInfo.ScreenNameResponse;

			await base.OnLoad();
		}

		public override Icon Icon => Icon.User;

		protected override Expression<Func<Status, bool>> StatusFilterExpression
									=> s => s.Type == StatusType.User && s.UserID == Context.UserId;

		private readonly ulong UserId;
	}
}