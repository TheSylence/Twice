using System;
using System.IO;

namespace Twice.Models.Proxy
{
	internal interface IHttpResponse
	{
		void SetCacheInfo( DateTimeOffset? expires, DateTimeOffset? lastModified );

		void SetContentInfo( long length, string type, byte[] md5 );

		void SetStatus( int code, string description );

		Stream OutputStream { get; }
	}
}