using System;
using System.IO;
using System.Net;

namespace Twice.Models.Proxy
{
	internal class HttpResponseImpl : IHttpResponse
	{
		public HttpResponseImpl( HttpListenerResponse response )
		{
			Response = response;
		}

		public void SetCacheInfo( DateTimeOffset? expires, DateTimeOffset? lastModified )
		{
			if( expires.HasValue )
			{
				Response.Headers.Add( HttpResponseHeader.Expires, expires.ToString() );
			}

			if( lastModified.HasValue )
			{
				Response.Headers.Add( HttpResponseHeader.LastModified, lastModified.ToString() );
			}
		}

		public void SetContentInfo( long length, string type, byte[] md5 )
		{
			Response.ContentLength64 = length;
			Response.ContentType = type;

			if( md5 != null )
			{
				Response.Headers.Add( HttpResponseHeader.ContentMd5, Convert.ToBase64String( md5 ) );
			}
		}

		public void SetStatus( int code, string description )
		{
			Response.StatusCode = code;
			Response.StatusDescription = description;
		}

		public Stream OutputStream => Response.OutputStream;
		private readonly HttpListenerResponse Response;
	}
}