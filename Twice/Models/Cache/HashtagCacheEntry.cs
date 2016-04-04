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
			return $"HASHTAG:{Hashtag}";
		}

		public string Hashtag { get; }
	}
}