using Anotar.NLog;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Twice.Utilities;

namespace Twice.Models.Proxy
{
	internal class MediaProxyServer : IDisposable
	{
		public MediaProxyServer( IHttpClient client = null, IHttpListener listener = null )
		{
			Client = client ?? new HttpClientWrapper( new HttpClient() );
			Http = listener ?? new HttpListenerWrapper( new HttpListener() );
		}

		public void Dispose()
		{
			Client.Dispose();
		}

		public void Start()
		{
			const string prefix = "http://localhost:60123/twice/media/";
			Http.AddPrefix( prefix );
			Http.Start();

			LogTo.Info( $"Starting proxy server on {prefix}" );
			IsRunning = true;
			ServerThread = new Thread( RunThreaded );
			ServerThread.Start();
		}

		public void Stop()
		{
			LogTo.Info( "Stopping media proxy server" );
			IsRunning = false;
			ServerThread?.Join();

			Http.Stop();
			Http.ClearPrefixes();
		}

		internal async Task HandleRequest( IHttpRequest request, IHttpResponse response )
		{
			if( request == null )
			{
				throw new ArgumentNullException( nameof( request ) );
			}
			if( response == null )
			{
				throw new ArgumentNullException( nameof( response ) );
			}

			var requestUrl = request.GetQueryParameter( "stream" );
			if( requestUrl != null )
			{
				var requestUri = new Uri( requestUrl );
				if( requestUri.Scheme.ToLower() == "https" )
				{
					LogTo.Debug( $"Proxy request for {requestUri}" );

					var get = await Client.GetAsync( requestUri );
					response.SetStatus( (int)get.StatusCode, get.ReasonPhrase );

					if( get.IsSuccessStatusCode )
					{
						var data = await get.Content.ReadAsByteArrayAsync();

						response.SetContentInfo( get.Content.Headers.ContentLength ?? 0, get.Content.Headers.ContentType.MediaType, get.Content.Headers.ContentMD5 );
						response.SetCacheInfo( get.Content.Headers.Expires, get.Content.Headers.LastModified );

						response.OutputStream.Write( data, 0, data.Length );
					}
					else
					{
						LogTo.Info( $"Remote server returned HTTP {(int)get.StatusCode} Code" );
					}
				}
				else
				{
					LogTo.Warn( "Proxy request for non HTTPS resource" );
				}
			}

			response.OutputStream.Close();
		}

		[ExcludeFromCodeCoverage]
		private async void RunThreaded()
		{
			while( IsRunning )
			{
				await Http.GetContextAsync().ContinueWith( async t =>
				{
					var request = new HttpRequestImpl( t.Result.Request );
					var response = new HttpResponseImpl( t.Result.Response );

					await HandleRequest( request, response );
				} );

				Thread.Sleep( 10 );
			}
		}

		private readonly IHttpClient Client;
		private readonly IHttpListener Http;
		private bool IsRunning;
		private Thread ServerThread;
	}
}