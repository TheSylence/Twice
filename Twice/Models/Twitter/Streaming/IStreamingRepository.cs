using System;
using Twice.Models.Columns;

namespace Twice.Models.Twitter.Streaming
{
	internal interface IStreamingRepository : IDisposable
	{
		IStreamParser GetParser( ColumnDefinition column );
	}
}