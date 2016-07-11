using System.Net;
using System.Threading.Tasks;

namespace Twice.Models.Proxy
{
	internal class HttpListenerWrapper : IHttpListener
	{
		public HttpListenerWrapper( HttpListener listener )
		{
			Listener = listener;
		}

		public void AddPrefix( string prefix )
		{
			Listener.Prefixes.Add( prefix );
		}

		public void ClearPrefixes()
		{
			Listener.Prefixes.Clear();
		}

		public Task<HttpListenerContext> GetContextAsync()
		{
			return Listener.GetContextAsync();
		}

		public void Start()
		{
			Listener.Start();
		}

		public void Stop()
		{
			Listener.Stop();
		}

		private readonly HttpListener Listener;
	}
}