﻿using System.Linq;
using System.Threading.Tasks;
using Anotar.NLog;
using Fody;
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

		public int MaxImageSize => Configuration?.PhotoSizeLimit ?? int.MaxValue;

		[ConfigureAwait( false )]
		public async Task QueryConfig()
		{
			LogTo.Info( "Reading current configuration from twitter" );

			var currentConfig = await Cache.ReadTwitterConfig();
			if( currentConfig == null )
			{
				LogTo.Info( "No current config cached. Fetching from server" );
				var ctx = ContextList.Contexts.FirstOrDefault();

				if( ctx != null )
				{
					currentConfig = await ctx.Twitter.GetConfig();
					await Cache.SaveTwitterConfig( currentConfig );
				}
			}
			else
			{
				LogTo.Debug( "Using configuration that was previously cached" );
			}

			Configuration = currentConfig;
		}

		public int UrlLength => Configuration?.ShortUrlLength ?? 23;
		public int UrlLengthHttps => Configuration?.ShortUrlLengthHttps ?? 23;

		private readonly ICache Cache;
		private readonly ITwitterContextList ContextList;
		private LinqToTwitter.Configuration Configuration;
	}
}