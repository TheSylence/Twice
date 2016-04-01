using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Anotar.NLog;
using LinqToTwitter;
using Twice.Models.Twitter;
using Twice.Resources;
using Twice.ViewModels.Columns.Definitions;

namespace Twice.ViewModels.Columns
{
	internal class TimelineColumn : ColumnViewModelBase
	{
		// TODO: Implement joined timelines for multiple contexts
		public TimelineColumn( IContextEntry context, ColumnDefinition definition )
			: base( context, definition )
		{
			Title = Strings.Home;
		}

		public override Icon Icon => Icon.Home;

		protected override Expression<Func<Status, bool>> StatusFilterExpression
			=> s => s.Type == StatusType.Home; // && s.UserID == Context.UserId;
	}
}