using System.Collections.Generic;
using LinqToTwitter;

namespace Twice.Models.Twitter.Comparers
{
	internal static class TwitterComparers
	{
		static TwitterComparers()
		{
			MediaEntityComparer = new MediaEntityComparer();
			UrlEntityComparer = new UrlEntityComparer();
			StatusComparer = new StatusComparer();
		}

		public static IEqualityComparer<MediaEntity> MediaEntityComparer { get; }
		public static IEqualityComparer<Status> StatusComparer { get; }
		public static IEqualityComparer<UrlEntity> UrlEntityComparer { get; }
	}
}