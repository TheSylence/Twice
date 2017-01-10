using GalaSoft.MvvmLight.Messaging;
using LinqToTwitter;
using Newtonsoft.Json;
using Seal.Fody;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Twice.Messages;
using Twice.Models.Cache;
using Twice.Models.Columns;
using Twice.Models.Configuration;
using Twice.Models.Twitter;
using Twice.Models.Twitter.Streaming;
using Twice.ViewModels.Twitter;

namespace Twice.ViewModels.Columns
{
	[LeaveUnsealed]
	internal class MessageColumn : ColumnViewModelBase
	{
		public MessageColumn( IContextEntry context, ColumnDefinition definition, IConfig config, IStreamParser parser,
			IMessenger messenger = null, IColumnActionDispatcher actionDispatcher = null )
			: base( context, definition, config, parser, messenger, actionDispatcher )
		{
			MessengerInstance.Register<DmMessage>( this, OnDirectMessage );
		}

		protected override bool IsSuitableForColumn( Status status )
		{
			return false;
		}

		protected override bool IsSuitableForColumn( DirectMessage message )
		{
			return Context.UserId == message.SenderID || Context.UserId == message.RecipientID;
		}

		protected override async Task OnLoad()
		{
			Dictionary<ulong, DirectMessage> userMap = new Dictionary<ulong, DirectMessage>();
			var cachedMessages = await Cache.GetMessages();
			foreach( var c in cachedMessages )
			{
				var userId = c.Sender == Context.UserId
					? c.Recipient
					: c.Sender;
				var msg = JsonConvert.DeserializeObject<DirectMessage>( c.Data );

				DirectMessage existing;
				if( userMap.TryGetValue( userId, out existing ) )
				{
					if( existing.CreatedAt < msg.CreatedAt )
					{
						userMap[userId] = msg;
					}
				}
				else
				{
					userMap.Add( userId, msg );
				}
			}

			var messages = await Context.Twitter.Messages.IncomingMessages( 200, MaxId );
			foreach( var msg in messages )
			{
				var userId = msg.SenderID;
				DirectMessage existing;
				if( userMap.TryGetValue( userId, out existing ) )
				{
					if( existing.CreatedAt < msg.CreatedAt )
					{
						userMap[userId] = msg;
					}
				}
				else
				{
					userMap.Add( userId, msg );
				}
			}

			var toCache = new List<MessageCacheEntry>();
			toCache.AddRange( messages.Select( m => new MessageCacheEntry( m ) ) );

			messages = await Context.Twitter.Messages.OutgoingMessages( 200, MaxId );
			foreach( var msg in messages )
			{
				var userId = msg.RecipientID;
				DirectMessage existing;
				if( userMap.TryGetValue( userId, out existing ) )
				{
					if( existing.CreatedAt < msg.CreatedAt )
					{
						userMap[userId] = msg;
					}
				}
				else
				{
					userMap.Add( userId, msg );
				}
			}

			toCache.AddRange( messages.Select( m => new MessageCacheEntry( m ) ) );
			await Cache.AddMessages( toCache );

			var list = new List<MessageViewModel>();
			foreach( var s in userMap.Values.OrderByDescending( m => m.CreatedAt ) )
			{
				var vm = await CreateViewModel( s );
				vm.WasRead = true;

				list.Add( vm );
			}

			await AddItems( list );
		}

		private async void OnDirectMessage( DmMessage msg )
		{
			if( msg.Action == EntityAction.Create )
			{
				var vm = await CreateViewModel( msg.DirectMessage );
				await AddItem( vm );
			}
		}

		public override Icon Icon => Icon.Messages;
		protected override Expression<Func<Status, bool>> StatusFilterExpression => s => s.Type == StatusType.User;
	}
}