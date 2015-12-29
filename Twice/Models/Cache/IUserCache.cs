using System.Collections.Generic;
using System.Threading.Tasks;

namespace Twice.Models.Cache
{
	internal interface IUserCache
	{
		Task Add( ulong id, string name );

		Task<IEnumerable<ulong>> GetKnownUsers();
	}
}