using LinqToTwitter;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Twice.Tests
{
	[ExcludeFromCodeCoverage]
	internal static class DummyGenerator
	{
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

		internal static User CreateDummyUser()
		{
			return new User
			{
				ProfileImageUrl = "http://example.com/image_normal.png",
				ProfileImageUrlHttps = "https://example.com/image_normal.png"
			};
		}
	}
}