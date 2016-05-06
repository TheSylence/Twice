using System;
using System.Linq.Expressions;
using LinqToTwitter;
using Twice.Models.Columns;
using Twice.Models.Configuration;
using Twice.Models.Twitter;
using Twice.Models.Twitter.Streaming;
using Twice.Resources;

namespace Twice.ViewModels.Columns
{
	internal class TimelineColumn : ColumnViewModelBase
	{
		// TODO: Implement joined timelines for multiple contexts
		public TimelineColumn( IContextEntry context, ColumnDefinition definition, IConfig config, IStreamParser parser )
			: base( context, definition, config, parser )
		{
			Title = Strings.Timeline;
		}

		protected override bool IsSuitableForColumn( Status status )
		{
			return true;
		}

		public override Icon Icon => Icon.Home;

		protected override Expression<Func<Status, bool>> StatusFilterExpression
			=> s => s.Type == StatusType.Home; // && s.UserID == Context.UserId;
	}
}