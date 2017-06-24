using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LinqToTwitter;

namespace Twice.Models.Twitter.Streaming
{
	internal interface IStreamingConnection
	{
		Task<List<IStreaming>> Start( Func<IStreamContent, Task> callback );
	}
}