using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twice.Models.Proxy;

namespace Twice.Tests.Models.Proxy
{
	[TestClass, ExcludeFromCodeCoverage]
	public class MediaProxyServerTests
	{
		[TestMethod, TestCategory( "Models.Proxy" )]
		public async Task DataIsCorrectlyWrittenToResponse()
		{
			// Arrange
			var responseMsg = new HttpResponseMessage( HttpStatusCode.OK );
			responseMsg.Content = new ByteArrayContent( new byte[] {1, 2, 3, 4} );
			responseMsg.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue( "video/mp4" );
			responseMsg.Content.Headers.ContentLength = 4;

			var client = new Mock<IHttpClient>();
			client.Setup( c => c.GetAsync( new Uri( "https://example.com" ) ) ).Returns( Task.FromResult( responseMsg ) );
			var proxy = new MediaProxyServer( client.Object );

			var request = new Mock<IHttpRequest>();
			request.Setup( c => c.GetQueryParameter( "stream" ) ).Returns( "https://example.com" );
			var responseStream = new MemoryStream();
			var response = new Mock<IHttpResponse>();
			response.SetupGet( c => c.OutputStream ).Returns( responseStream );
			response.Setup( c => c.SetContentInfo( 4, "video/mp4", It.IsAny<byte[]>() ) );

			// Act
			await proxy.HandleRequest( request.Object, response.Object );

			// Assert
			CollectionAssert.AreEqual( new byte[] {1, 2, 3, 4}, responseStream.ToArray() );
			response.Verify( c => c.SetContentInfo( 4, "video/mp4", It.IsAny<byte[]>() ), Times.Once() );
		}

		[TestMethod, TestCategory( "Models.Proxy" )]
		public void HttpClientIsDisposed()
		{
			// Arrange
			var client = new Mock<IHttpClient>();
			client.Setup( c => c.Dispose() ).Verifiable();
			var proxy = new MediaProxyServer( client.Object );

			// Act
			proxy.Dispose();

			// Assert
			client.Verify( c => c.Dispose(), Times.Once() );
		}

		[TestMethod, TestCategory( "Models.Proxy" )]
		public async Task MissingArgumentThrowsException()
		{
			// Arrange
			var client = new Mock<IHttpClient>();
			var proxy = new MediaProxyServer( client.Object );

			var request = new Mock<IHttpRequest>();
			var response = new Mock<IHttpResponse>();

			// Act
			var respEx = await ExceptionAssert.CatchAsync<ArgumentNullException>( () => proxy.HandleRequest( request.Object, null ) );
			var reqEx = await ExceptionAssert.CatchAsync<ArgumentNullException>( () => proxy.HandleRequest( null, response.Object ) );

			// Assert
			Assert.IsNotNull( respEx );
			Assert.IsNotNull( reqEx );
			Assert.AreEqual( "response", respEx.ParamName );
			Assert.AreEqual( "request", reqEx.ParamName );
		}

		[TestMethod, TestCategory( "Models.Proxy" )]
		public async Task MissingQueryParameterIsIgnored()
		{
			// Arrange
			var client = new Mock<IHttpClient>();
			client.Setup( c => c.GetAsync( It.IsAny<Uri>() ) ).Verifiable();
			var proxy = new MediaProxyServer( client.Object );

			var request = new Mock<IHttpRequest>();
			request.Setup( c => c.GetQueryParameter( "stream" ) ).Returns<string>( null );

			var response = new Mock<IHttpResponse>();
			response.SetupGet( c => c.OutputStream ).Returns( new MemoryStream() );

			// Act
			await proxy.HandleRequest( request.Object, response.Object );

			// Assert
			client.Verify( c => c.GetAsync( It.IsAny<Uri>() ), Times.Never() );
		}

		[TestMethod, TestCategory( "Models.Proxy" )]
		public async Task NotFoundIsPropagatedCorrectly()
		{
			// Arrange
			var responseMsg = new HttpResponseMessage( HttpStatusCode.NotFound );
			var client = new Mock<IHttpClient>();
			client.Setup( c => c.GetAsync( new Uri( "https://example.com" ) ) ).Returns( Task.FromResult( responseMsg ) );

			var proxy = new MediaProxyServer( client.Object );

			var request = new Mock<IHttpRequest>();
			request.Setup( c => c.GetQueryParameter( "stream" ) ).Returns( "https://example.com" );

			var response = new Mock<IHttpResponse>();
			response.Setup( c => c.SetStatus( 404, It.IsAny<string>() ) ).Verifiable();
			response.SetupGet( c => c.OutputStream ).Returns( new MemoryStream() );

			// Act
			await proxy.HandleRequest( request.Object, response.Object );

			// Assert
			response.Verify( c => c.SetStatus( 404, It.IsAny<string>() ), Times.Once() );
		}

		[TestMethod, TestCategory( "Models.Proxy" )]
		public void StartingAddsPrefix()
		{
			// Arrange
			var listener = new Mock<IHttpListener>();
			listener.Setup( c => c.AddPrefix( It.IsAny<string>() ) ).Verifiable();
			var proxy = new MediaProxyServer( null, listener.Object );

			// Act
			try
			{
				proxy.Start();
			}
			finally
			{
				proxy.Stop();
			}

			// Assert
			listener.Verify( c => c.AddPrefix( It.IsAny<string>() ), Times.Once() );
		}

		[TestMethod, TestCategory( "Models.Proxy" )]
		public void StartingStartsListener()
		{
			// Arrange
			var listener = new Mock<IHttpListener>();
			listener.Setup( c => c.Start() ).Verifiable();
			var proxy = new MediaProxyServer( null, listener.Object );

			// Act
			try
			{
				proxy.Start();
			}
			finally
			{
				proxy.Stop();
			}

			// Assert
			listener.Verify( c => c.Start(), Times.Once() );
		}

		[TestMethod, TestCategory( "Models.Proxy" )]
		public void StoppingRemovesPrefix()
		{
			// Arrange
			var listener = new Mock<IHttpListener>();
			listener.Setup( c => c.ClearPrefixes() ).Verifiable();

			var proxy = new MediaProxyServer( null, listener.Object );

			// Act
			proxy.Stop();

			// Assert
			listener.Verify( c => c.ClearPrefixes(), Times.Once() );
		}

		[TestMethod, TestCategory( "Models.Proxy" )]
		public void StoppingStopsListener()
		{
			// Arrange
			var listener = new Mock<IHttpListener>();
			listener.Setup( c => c.Stop() ).Verifiable();

			var proxy = new MediaProxyServer( null, listener.Object );

			// Act
			proxy.Stop();

			// Assert
			listener.Verify( c => c.Stop(), Times.Once() );
		}
	}
}