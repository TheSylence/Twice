using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using LinqToTwitter;
using Twice.Models.Twitter.Entities;

namespace Twice.Tests
{
	[ExcludeFromCodeCoverage]
	internal static class DummyGenerator
	{
		internal static DirectMessage CreateDummyMessage( User sender = null, User recipient = null )
		{
			sender = sender ?? CreateDummyUser();
			recipient = recipient ?? CreateDummyUser();

			return new DirectMessage
			{
				Sender = sender,
				Recipient = recipient,
				Entities = new Entities
				{
					HashTagEntities = new List<HashTagEntity>(),
					MediaEntities = new List<MediaEntity>(),
					SymbolEntities = new List<SymbolEntity>(),
					UrlEntities = new List<UrlEntity>(),
					UserMentionEntities = new List<UserMentionEntity>()
				}
			};
		}

		internal static Status CreateDummyStatus( User user = null )
		{
			user = user ?? CreateDummyUser();

			return new Status
			{
				User = user,
				Entities = new Entities
				{
					HashTagEntities = new List<HashTagEntity>(),
					MediaEntities = new List<MediaEntity>(),
					SymbolEntities = new List<SymbolEntity>(),
					UrlEntities = new List<UrlEntity>(),
					UserMentionEntities = new List<UserMentionEntity>()
				},
				ExtendedEntities = new Entities
				{
					HashTagEntities = new List<HashTagEntity>(),
					MediaEntities = new List<MediaEntity>(),
					SymbolEntities = new List<SymbolEntity>(),
					UrlEntities = new List<UrlEntity>(),
					UserMentionEntities = new List<UserMentionEntity>()
				}
			};
		}

		internal static User CreateDummyUser( ulong? userId = null )
		{
			return CreateDummyUserEx( userId );
		}

		internal static UserEx CreateDummyUserEx( ulong? userId = null )
		{
			return new UserEx
			{
				ProfileImageUrl = "http://example.com/image_normal.png",
				ProfileImageUrlHttps = "https://example.com/image_normal.png",
				UserID = userId ?? 0,
				UserIDResponse = ( userId ?? 0 ).ToString()
			};
		}
	}
}