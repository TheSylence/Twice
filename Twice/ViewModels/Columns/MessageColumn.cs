using LinqToTwitter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Twice.Models.Columns;
using Twice.Models.Configuration;
using Twice.Models.Twitter;
using Twice.Models.Twitter.Streaming;
using System.Threading.Tasks;
using Twice.ViewModels.Twitter;

namespace Twice.ViewModels.Columns
{
	internal class MessageColumn : ColumnViewModelBase
	{
		public MessageColumn( IContextEntry context, ColumnDefinition definition, IConfig config, IStreamParser parser ) : base( context, definition, config, parser )
		{
		}

		protected override async Task OnLoad()
		{
			var messages = await Context.Twitter.Messages.IncomingMessages();
			messages.AddRange( await Context.Twitter.Messages.OutgoingMessages() );
			var list = new List<MessageViewModel>();
			foreach( var s in messages.OrderByDescending( m => m.CreatedAt ) )
			{
				list.Add( await CreateViewModel( s ) );
			}

			await AddItems( list );
		}

		protected override bool IsSuitableForColumn( Status status )
		{
			return false;
		}

		protected override bool IsSuitableForColumn( DirectMessage message )
		{
			// TODO: Check context for user ids
			return true;
		}

		public override Icon Icon => Icon.Messages;
		protected override Expression<Func<Status, bool>> StatusFilterExpression => s => false;
	}
}