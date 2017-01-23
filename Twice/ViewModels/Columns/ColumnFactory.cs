using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using Twice.Models.Cache;
using Twice.Models.Columns;
using Twice.Models.Configuration;
using Twice.Models.Scheduling;
using Twice.Models.Twitter;
using Twice.Models.Twitter.Streaming;
using Twice.Utilities.Ui;
using Twice.Views.Services;

namespace Twice.ViewModels.Columns
{
	internal class ColumnFactory : IColumnFactory
	{
		public ColumnFactory()
		{
			FactoryMap = new Dictionary<ColumnType, Func<ColumnArgumentsData, ColumnViewModelBase>>
			{
				{ColumnType.User, UserColumn},
				{ColumnType.Timeline, TimelineColumn},
				{ColumnType.Mentions, MentionsColumn},
				{ColumnType.Messages, MessageColumn},
				{ColumnType.Favorites, FavoritesColumn},
				{ColumnType.Schedule, ScheduleColumn},
				{ColumnType.DebugOrTest, StaticColumn}
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
					Parser = StreamingRepo.GetParser( def ),
					Scheduler = Scheduler
				};

				var column = factoryAction( argData );

				column.Width = def.Width;
				column.Muter = Muter;
				column.Cache = Cache;
				column.Dispatcher = Dispatcher;
				column.ViewServiceRepository = ViewServiceRepository;

				return column;
			}

			return null;
		}

		private ColumnViewModelBase FavoritesColumn( ColumnArgumentsData args )
		{
			return new FavoritesColumn( args.Context, args.Definition, args.Configuration, args.Parser );
		}

		private ColumnViewModelBase MentionsColumn( ColumnArgumentsData args )
		{
			return new MentionsColumn( args.Context, args.Definition, args.Configuration, args.Parser );
		}

		private ColumnViewModelBase MessageColumn( ColumnArgumentsData args )
		{
			return new MessageColumn( args.Context, args.Definition, args.Configuration, args.Parser );
		}

		private ColumnViewModelBase ScheduleColumn( ColumnArgumentsData args )
		{
			return new ScheduleColumn( args.Context, args.Definition, args.Configuration, args.Parser, args.Scheduler );
		}

		private ColumnViewModelBase StaticColumn( ColumnArgumentsData args )
		{
			return new StaticColumn( args.Context, args.Definition, args.Configuration, args.Parser );
		}

		private ColumnViewModelBase TimelineColumn( ColumnArgumentsData args )
		{
			return new TimelineColumn( args.Context, args.Definition, args.Configuration, args.Parser );
		}

		private ColumnViewModelBase UserColumn( ColumnArgumentsData args )
		{
			return new UserColumn( args.Context, args.Definition, args.Configuration, args.Parser );
		}

		[Inject]
		// ReSharper disable once MemberCanBePrivate.Global
		public ICache Cache { get; set; }

		[Inject]
		public IConfig Configuration { get; set; }

		[Inject]
		public ITwitterContextList Contexts { get; set; }

		[Inject]
		// ReSharper disable once MemberCanBePrivate.Global
		public IDispatcher Dispatcher { get; set; }

		[Inject]
		// ReSharper disable once MemberCanBePrivate.Global
		public IStatusMuter Muter { get; set; }

		[Inject]
		// ReSharper disable once MemberCanBePrivate.Global
		public IScheduler Scheduler { get; set; }

		[Inject]
		public IStreamingRepository StreamingRepo { get; set; }

		[Inject]
		// ReSharper disable once MemberCanBePrivate.Global
		public IViewServiceRepository ViewServiceRepository { get; set; }

		private readonly Dictionary<ColumnType, Func<ColumnArgumentsData, ColumnViewModelBase>> FactoryMap;

		private class ColumnArgumentsData
		{
			public IConfig Configuration;
			public IContextEntry Context;
			public ColumnDefinition Definition;
			public IStreamParser Parser;
			public IScheduler Scheduler;
		}
	}
}