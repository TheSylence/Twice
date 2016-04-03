using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twice.Models.Columns;

namespace Twice.Models.Twitter
{
	interface IStreamingRepository : IDisposable
	{
		IStreamParser GetParser( ColumnDefinition column );
	}
}
