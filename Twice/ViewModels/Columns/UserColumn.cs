using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LinqToTwitter;
using Twice.Models.Twitter;
using Twice.ViewModels.Columns.Definitions;

namespace Twice.ViewModels.Columns
{
	internal class UserColumn : ColumnViewModelBase
	{
		public UserColumn( IContextEntry context, ColumnDefinition definition, ulong userId )
			: base( context, definition )
		{
			UserId = userId;
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