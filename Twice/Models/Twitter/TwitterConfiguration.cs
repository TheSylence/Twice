using Anotar.NLog;
using System.Linq;
using System.Threading.Tasks;
using Twice.Models.Cache;

namespace Twice.Models.Twitter
{
	internal class TwitterConfiguration : ITwitterConfiguration
	{
		public TwitterConfiguration( ICache cache, ITwitterContextList contextList )
		{
			ContextList = contextList;
			Cache = cache;
		}

		public async Task QueryConfig()
		{
			LogTo.Info( "Reading current configuration from twitter" );

			var currentConfig = await Cache.ReadTwitterConfig();
			if( currentConfig == null )
			{
				LogTo.Info( "No current config cached. Fetching from server" );
				var ctx = ContextList.Contexts.First();
				currentConfig = await ctx.Twitter.GetConfig();

				await Cache.SaveTwitterConfig( currentConfig );
			}
			else
			{
				LogTo.Debug( "Using configuration that was previously cached" );
			}

			Configuration = currentConfig;
		}

		public int MaxImageSize => Configuration?.PhotoSizeLimit ?? int.MaxValue;
		public int UrlLength => Configuration?.ShortUrlLength ?? 23;
		public int UrlLengthHttps => Configuration?.ShortUrlLengthHttps ?? 23;

		private readonly ICache Cache;
		private readonly ITwitterContextList ContextList;
		private LinqToTwitter.Configuration Configuration;
	}
}