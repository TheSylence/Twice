using System;
using System.Collections.Generic;
using System.Linq;
using Twice.Models.Cache;
using Twice.Models.Columns;
using Twice.Models.Configuration;
using Twice.Models.Twitter;

namespace Twice.ViewModels.Columns
{
	internal class ColumnFactory : IColumnFactory
	{
		public ColumnFactory( ITwitterContextList contexts, IStatusMuter muter, IConfig config, IStreamingRepository streamingRepo,
			IDataCache cache )
		{
			Cache = cache;
			Configuration = config;
			Muter = muter;
			Contexts = contexts;
			StreamingRepo = streamingRepo;

			FactoryMap = new Dictionary<ColumnType, Func<ColumnArgumentsData, ColumnViewModelBase>>
			{
				{ColumnType.User, UserColumn},
				{ColumnType.Timeline, TimelineColumn},
				{ColumnType.Mentions, MentionsColumn}
			};
		}

		public ColumnViewModelBase Construct( ColumnDefinition def )
		{
			// TODO: We need all contexts if SourceAccounts contains more than one id
			var context = Contexts.Contexts.FirstOrDefault( c => def.SourceAccounts.Contains( c.UserId ) );
			if( context == null )
			{
				return null;
			}

			Func<ColumnArgumentsData, ColumnViewModelBase> factoryAction;

			if( FactoryMap.TryGetValue( def.Type, out factoryAction ) )
			{
				var argData = new ColumnArgumentsData
				{
					Configuration = Configuration,
					Context = context,
					Definition = def,
					Parser = StreamingRepo.GetParser( def )
				};

				var column = factoryAction( argData );

				column.Width = def.Width;
				column.Muter = Muter;
				column.Cache = Cache;

				return column;
			}

			return null;
		}

		private ColumnViewModelBase MentionsColumn( ColumnArgumentsData args )
		{
			return new MentionsColumn( args.Context, args.Definition, args.Configuration, args.Parser );
		}

		private ColumnViewModelBase TimelineColumn( ColumnArgumentsData args )
		{
			return new TimelineColumn( args.Context, args.Definition, args.Configuration, args.Parser );
		}

		private ColumnViewModelBase UserColumn( ColumnArgumentsData args )
		{
			return new UserColumn( args.Context, args.Definition, args.Configuration, args.Parser );
		}

		private readonly IDataCache Cache;
		private readonly IConfig Configuration;
		private readonly ITwitterContextList Contexts;
		private readonly Dictionary<ColumnType, Func<ColumnArgumentsData, ColumnViewModelBase>> FactoryMap;
		private readonly IStatusMuter Muter;
		private readonly IStreamingRepository StreamingRepo;

		private class ColumnArgumentsData
		{
			public IConfig Configuration;
			public IContextEntry Context;
			public ColumnDefinition Definition;
			public IStreamParser Parser;
		}
	}
}