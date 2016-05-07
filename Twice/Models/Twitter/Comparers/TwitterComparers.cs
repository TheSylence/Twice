using System.Collections.Generic;
using LinqToTwitter;

namespace Twice.Models.Twitter.Comparers
{
	internal static class TwitterComparers
	{
		static TwitterComparers()
		{
			MediaEntityComparer = new MediaEntityComparer();
		}

		public static IEqualityComparer<MediaEntity> MediaEntityComparer { get; }
	}
}