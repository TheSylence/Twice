using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twice.Models.Twitter
{
	class RetweetOptions
	{
		public string QuoteText { get; }
		public bool Quote { get; }
		public ulong StatusId { get; }
	}
}
