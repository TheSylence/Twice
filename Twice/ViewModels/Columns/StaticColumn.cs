using GalaSoft.MvvmLight.Messaging;
using LinqToTwitter;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Twice.Models.Columns;
using Twice.Models.Configuration;
using Twice.Models.Twitter;
using Twice.Models.Twitter.Streaming;
using Twice.ViewModels.Twitter;

namespace Twice.ViewModels.Columns
{
	[ExcludeFromCodeCoverage]
	internal class StaticColumn : ColumnViewModelBase
	{
		public StaticColumn( IContextEntry context, ColumnDefinition definition, IConfig config, IStreamParser parser,
			IMessenger messenger = null )
			: base( context, definition, config, parser, messenger )
		{
		}

		protected override bool IsSuitableForColumn( Status status )
		{
			return false;
		}

		protected override bool IsSuitableForColumn( DirectMessage message )
		{
			return false;
		}

		protected override async Task OnLoad( AsyncLoadContext context )
		{
			var statuses = await Context.Twitter.Statuses.List( Definition.TargetAccounts );
			var list = new List<StatusViewModel>();
			foreach( var s in statuses )
			{
				list.Add( await CreateViewModel( s, context ) );
			}

			await AddItems( list );
		}

		public override Icon Icon => Icon.User;
		protected override Expression<Func<Status, bool>> StatusFilterExpression => s => true;
	}
}