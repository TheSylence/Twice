using System.Threading.Tasks;

namespace Twice.Models.Twitter
{
	internal interface ITwitterConfiguration
	{
		Task QueryConfig();

		int UrlLength { get; }
		int UrlLengthHttps { get; }
	}
}