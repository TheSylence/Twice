using Akavache;

namespace Twice.Models.Cache
{
	internal class DataCache : IDataCache
	{
		public DataCache( ISecureBlobCache secure, IBlobCache data )
		{
			Secure = secure;
			Data = data;

			Users = new UserCache( data );
		}

		public IBlobCache Data { get; }
		public ISecureBlobCache Secure { get; }
		public IUserCache Users { get; }
	}
}