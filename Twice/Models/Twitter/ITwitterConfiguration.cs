using System.Threading.Tasks;

namespace Twice.Models.Twitter
{
	internal interface ITwitterConfiguration
	{
		Task QueryConfig();

		int MaxImageSize { get; }
		int UrlLength { get; }
		int UrlLengthHttps { get; }
	}
}