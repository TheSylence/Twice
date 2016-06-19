using LinqToTwitter;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Regex = Twice.Models.Twitter.Text.Regex;

namespace Twice.Models.Twitter.Entities
{
	internal class EntityParser
	{
		internal static List<HashTagEntity> ExtractHashtags( string text )
		{
			var hashtags = new List<HashTagEntity>();

			foreach( Match match in Regex.VALID_HASHTAG.Matches( text ) )
			{
				int start = match.Index + 1;
				int end = start + match.Length - 1;
				if( start == 1 )
				{
					// FIXME: I have no idea why...
					start = 0;
				}

				HashTagEntity tag = new HashTagEntity
				{
					Start = start,
					End = end,
					Tag = match.Groups[3].Value
				};

				hashtags.Add( tag );
			}

			return hashtags;
		}
	}
}