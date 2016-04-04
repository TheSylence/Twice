namespace Twice.Models.Cache
{
	internal class UserCacheEntry : ICacheEntry
	{
		public UserCacheEntry( ulong id, string name )
		{
			Id = id;
			Name = name;
		}

		public string GetKey()
		{
			return $"USER:{Id}";
		}

		public ulong Id { get; }
		public string Name { get; }
	}
}