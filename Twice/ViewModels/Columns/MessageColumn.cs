﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LinqToTwitter;
using Newtonsoft.Json;
using Twice.Messages;
using Twice.Models.Cache;
using Twice.Models.Columns;
using Twice.Models.Configuration;
using Twice.Models.Twitter;
using Twice.Models.Twitter.Streaming;
using Twice.ViewModels.Twitter;

namespace Twice.ViewModels.Columns
{
	internal class MessageColumn : ColumnViewModelBase
	{
		public MessageColumn( IContextEntry context, ColumnDefinition definition, IConfig config, IStreamParser parser )
			: base( context, definition, config, parser )
		{
			MessengerInstance.Register<DmMessage>( this, OnDirectMessage );
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

			await Cache.AddMessages( messages.Select( m => new MessageCacheEntry( m ) ).ToList() );

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

			await Cache.AddMessages( messages.Select( m => new MessageCacheEntry( m ) ).ToList() );

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
				await AddItem( vm, false );
			}
		}

		public override Icon Icon => Icon.Messages;
		protected override Expression<Func<Status, bool>> StatusFilterExpression => s => false;
		private readonly ulong MaxId = ulong.MaxValue;
	}
}