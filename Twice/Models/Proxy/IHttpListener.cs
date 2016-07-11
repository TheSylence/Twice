using System.Net;
using System.Threading.Tasks;

namespace Twice.Models.Proxy
{
	internal interface IHttpListener
	{
		void AddPrefix( string prefix );

		void ClearPrefixes();

		Task<HttpListenerContext> GetContextAsync();

		void Start();

		void Stop();
	}
}