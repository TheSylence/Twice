using System;
using System.Net.Http;
using System.Threading.Tasks;
using Twice.Models.Proxy;

namespace Twice.Utilities
{
	internal class HttpClientWrapper : IHttpClient
	{
		public HttpClientWrapper( HttpClient client )
		{
			Client = client;
		}

		public void Dispose()
		{
			Client.Dispose();
		}

		public Task<HttpResponseMessage> GetAsync( Uri url )
		{
			return Client.GetAsync( url );
		}

		private readonly HttpClient Client;
	}
}