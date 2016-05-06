using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;

namespace Twice.Models.Twitter.Streaming
{
	[ExcludeFromCodeCoverage]
	internal class StreamingConnection : IStreamingConnection
	{
		public StreamingConnection( IQueryable<LinqToTwitter.Streaming> obj )
		{
			Wrapped = obj;
		}

		public async Task<List<IStreaming>> Start( Func<IStreamContent, Task> callback )
		{
			var toWrap = await Wrapped.StartAsync( callback );

			return toWrap.Select( t => new StreamingWrapper( t ) ).Cast<IStreaming>().ToList();
		}

		private readonly IQueryable<LinqToTwitter.Streaming> Wrapped;
	}
}