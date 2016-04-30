using System.Diagnostics.CodeAnalysis;

namespace Twice.Models.Twitter.Streaming
{
	[ExcludeFromCodeCoverage]
	internal class StreamingWrapper : IStreaming
	{
		public StreamingWrapper( LinqToTwitter.Streaming obj )
		{
			Wrapped = obj;
		}

		public void CloseStream()
		{
			Wrapped.CloseStream();
		}

		private readonly LinqToTwitter.Streaming Wrapped;
	}
}