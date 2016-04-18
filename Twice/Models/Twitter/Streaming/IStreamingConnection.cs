using LinqToTwitter;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Twice.Models.Twitter.Streaming
{
	internal interface IStreamingConnection
	{
		Task<List<IStreaming>> Start( Func<StreamContent, Task> callback );
	}
}