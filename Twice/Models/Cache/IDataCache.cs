using Akavache;

namespace Twice.Models.Cache
{
	interface IDataCache
	{
		ISecureBlobCache Secure { get; }
		IBlobCache Data { get; }
	}
}