using System;
using System.Threading.Tasks;

namespace Twice.Models.Media
{
	internal interface ITwitterCardExtractor
	{
		Task<TwitterCard> ExtractCard( Uri url );
	}
}