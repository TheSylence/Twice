using System.Net;

namespace Twice.Models.Proxy
{
	internal class HttpRequestImpl : IHttpRequest
	{
		public HttpRequestImpl( HttpListenerRequest request )
		{
			Request = request;
		}

		public string GetQueryParameter( string param )
		{
			return Request.QueryString[param];
		}

		private readonly HttpListenerRequest Request;
	}
}