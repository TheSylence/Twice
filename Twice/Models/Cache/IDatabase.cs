using System;
using System.Threading.Tasks;

namespace Twice.Models.Cache
{
	interface IDatabase
	{
		Task Insert( string key, string data, DateTime? expiryDate = null );
		Task<bool> Exists( string key );
		Task<string> Read( string key );
	}
}