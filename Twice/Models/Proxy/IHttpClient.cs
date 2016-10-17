using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Twice.Models.Proxy
{
	internal interface IHttpClient : IDisposable
	{
		Task<HttpResponseMessage> GetAsync( Uri url, string auth = null );
	}
}