using System;
using System.Linq.Expressions;
using LinqToTwitter;
using Twice.Models.Twitter;
using Twice.Resources;
using Twice.ViewModels.Columns.Definitions;

namespace Twice.ViewModels.Columns
{
	internal class MentionsColumn : ColumnViewModelBase
	{
		public MentionsColumn( IContextEntry context, ColumnDefinition definition ) : base( context, definition )
		{
			Icon = Icon.Mentions;
			Title = Strings.Mentions;
		}

		public override Icon Icon { get; }

		protected override Expression<Func<Status, bool>> StatusFilterExpression
					=> s => s.Type == StatusType.Mentions && s.UserID == Context.UserId;
	}
}