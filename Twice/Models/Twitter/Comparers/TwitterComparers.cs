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
		}

		public static IEqualityComparer<MediaEntity> MediaEntityComparer { get; }
		public static IEqualityComparer<UrlEntity> UrlEntityComparer { get; }
	}
}