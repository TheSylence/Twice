using System;
using System.Linq;
using System.Linq.Expressions;
using LinqToTwitter;
using Twice.Models.Columns;
using Twice.Models.Configuration;
using Twice.Models.Twitter;
using Twice.Resources;

namespace Twice.ViewModels.Columns
{
	internal class MentionsColumn : ColumnViewModelBase
	{
		public MentionsColumn( IContextEntry context, ColumnDefinition definition, IConfig config, IStreamParser parser )
			: base( context, definition, config, parser )
		{
			Icon = Icon.Mentions;
			Title = Strings.Mentions;
		}

		protected override bool IsSuitableForColumn( Status status )
		{
			return status.Entities.UserMentionEntities.Any( m => m.ScreenName == Context.AccountName );
		}

		public override Icon Icon { get; }

		protected override Expression<Func<Status, bool>> StatusFilterExpression
					=> s => s.Type == StatusType.Mentions && s.UserID == Context.UserId;
	}
}