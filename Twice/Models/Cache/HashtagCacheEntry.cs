namespace Twice.Models.Cache
{
	internal class HashtagCacheEntry : ICacheEntry
	{
		public HashtagCacheEntry( string hashtag )
		{
			Hashtag = hashtag;
		}

		public string GetKey()
		{
			return $"{Key}:{Hashtag}";
		}

		public const string Key = "HASHTAG";

		public string Hashtag { get; }
	}
}