using Fody;
using LinqToTwitter;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Twice.Models.Cache;

namespace Twice.Models.Twitter.Repositories
{
	[ExcludeFromCodeCoverage]
	[ConfigureAwait( false )]
	internal class TwitterMessageRepository : TwitterRepositoryBase, ITwitterMessageRepository
	{
		public TwitterMessageRepository( TwitterContext context, ICache cache ) : base( context, cache )
		{
		}

		public async Task<List<DirectMessage>> IncomingMessages()
		{
			var list = await Queryable.Where( dm => dm.Type == DirectMessageType.SentTo ).ToListAsync();
			var result = new List<DirectMessage>( list.Count );
			foreach( var dm in list )
			{
				if( dm?.Recipient != null )
				{
					result.Add( dm );
				}
			}
			return result;
		}

		public async Task<List<DirectMessage>> OutgoingMessages()
		{
			var list = await Queryable.Where( dm => dm.Type == DirectMessageType.SentBy ).ToListAsync();
			var result = new List<DirectMessage>( list.Count );
			foreach( var dm in list )
			{
				if( dm?.Recipient != null )
				{
					result.Add( dm );
				}
			}
			return result;
		}

		private TwitterQueryable<DirectMessage> Queryable => Context.DirectMessage;
	}
}