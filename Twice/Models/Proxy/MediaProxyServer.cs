using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Anotar.NLog;
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

		public void Start()
		{
			try
			{
				Http.AddPrefix( Prefix );
				Http.Start();

				LogTo.Info( $"Starting proxy server on {Prefix}" );
				IsRunning = true;
				ServerThread = new Thread( RunThreaded );
				ServerThread.Start();
			}
			catch( HttpListenerException ex )
			{
				LogTo.ErrorException( "Failed to start media proxy server", ex );
			}
		}

		public void Stop()
		{
			LogTo.Info( "Stopping media proxy server" );
			IsRunning = false;
			ServerThread?.Join();

			try
			{
				Http.Stop();
				Http.ClearPrefixes();
			}
			catch( Exception ex )
			{
				LogTo.WarnException( "Error while shutting down http listener", ex );
			}
		}

		internal static Uri BuildUrl( string url )
		{
			if( url.StartsWith( "https://", StringComparison.OrdinalIgnoreCase ) )
			{
				var encoded = Uri.EscapeUriString( url );
				return new Uri( $"{Prefix}?stream={encoded}" );
			}

			return new Uri( url );
		}

		internal static Uri BuildUrl( Uri url )
		{
			return BuildUrl( url.AbsoluteUri );
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

						var contentLength = get.Content.Headers.ContentLength ?? 0;
						var contentType = get.Content.Headers.ContentType.MediaType;
						var contentMd5 = get.Content.Headers.ContentMD5;

						LogTo.Debug( $"Content-Length: {contentLength}" );
						LogTo.Debug( $"Content-Type: {contentType}" );
						LogTo.Debug( $"Content-MD5: {contentMd5}" );

						response.SetContentInfo( contentLength, contentType, contentMd5 );
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

		public void Dispose()
		{
			Client.Dispose();
		}

		private const string Prefix = "http://localhost:60123/twice/media/";
		private readonly IHttpClient Client;
		private readonly IHttpListener Http;
		private bool IsRunning;
		private Thread ServerThread;
	}
}