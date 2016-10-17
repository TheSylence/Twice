using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Anotar.NLog;
using Twice.Models.Twitter;
using Twice.Utilities;

namespace Twice.Models.Proxy
{
	internal class MediaProxyServer : IDisposable
	{
		public MediaProxyServer( IHttpClient client = null, IHttpListener listener = null, ITwitterContextList contextList = null )
		{
			ContextList = contextList;
			Client = client ?? new HttpClientWrapper( new HttpClient() );
			Http = listener ?? new HttpListenerWrapper( new HttpListener() );
		}

		public void Dispose()
		{
			Client.Dispose();
		}

		public void Start( ITwitterContextList contextList = null )
		{
			try
			{
				ContextList = contextList;
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

		internal static Uri BuildUrl( string url, ulong userId = 0 )
		{
			if( url.StartsWith( "https://", StringComparison.OrdinalIgnoreCase ) )
			{
				var encoded = Uri.EscapeUriString( url );
				string request = $"{Prefix}?stream={encoded}";
				if( userId != 0 )
				{
					request += $"&user={userId}";
				}

				return new Uri( request );
			}

			return new Uri( url );
		}

		internal static Uri BuildUrl( Uri url, ulong userId = 0 )
		{
			return BuildUrl( url.AbsoluteUri, userId );
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
				string authHeader = string.Empty;
				var userIdParam = request.GetQueryParameter( "user" );
				ulong userId;
				if( ulong.TryParse( userIdParam, out userId ) )
				{
					var context = GetContext( userId );
					if( context != null )
					{
						authHeader = context.Twitter.Authorizer.GetAuthorizationString( "GET", requestUrl, new Dictionary<string, string>() );
					}
					else
					{
						LogTo.Warn( $"No information found for user {userId}" );
					}
				}

				var requestUri = new Uri( requestUrl );
				if( requestUri.Scheme.ToLower() == "https" )
				{
					LogTo.Debug( $"Proxy request for {requestUri}" );

					var get = await Client.GetAsync( requestUri, authHeader );
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

		private IContextEntry GetContext( ulong userId )
		{
			return ContextList?.Contexts?.FirstOrDefault( ctx => ctx.UserId == userId );
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

		private const string Prefix = "http://localhost:60123/twice/media/";
		private readonly IHttpClient Client;
		private readonly IHttpListener Http;
		private ITwitterContextList ContextList;
		private bool IsRunning;
		private Thread ServerThread;
	}
}