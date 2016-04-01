using System;
using System.Collections.Generic;
using System.Linq;
using Twice.Models.Configuration;
using Twice.Models.Twitter;
using Twice.ViewModels.Columns.Definitions;

namespace Twice.ViewModels.Columns
{
	internal class ColumnFactory
	{
		public ColumnFactory( ITwitterContextList contexts, IStatusMuter muter, IConfig config )
		{
			Configuration = config;
			Muter = muter;
			Contexts = contexts;

			FactoryMap = new Dictionary<ColumnType, Func<IContextEntry, ColumnDefinition, ColumnViewModelBase>>
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

			Func<IContextEntry, ColumnDefinition, ColumnViewModelBase> factoryAction;

			if( FactoryMap.TryGetValue( def.Type, out factoryAction ) )
			{
				var column = factoryAction( context, def );

				column.Width = def.Width;
				column.Muter = Muter;
				column.Configuration = Configuration;

				return column;
			}

			return null;
		}

		private ColumnViewModelBase MentionsColumn( IContextEntry context, ColumnDefinition definition )
		{
			return new MentionsColumn( context, definition );
		}

		private ColumnViewModelBase TimelineColumn( IContextEntry context, ColumnDefinition definition )
		{
			return new TimelineColumn( context, definition );
		}

		private ColumnViewModelBase UserColumn( IContextEntry context, ColumnDefinition definition )
		{
			return new UserColumn( context, definition, definition.TargetAccounts.First() );
		}

		private readonly IConfig Configuration;
		private readonly ITwitterContextList Contexts;
		private readonly Dictionary<ColumnType, Func<IContextEntry, ColumnDefinition, ColumnViewModelBase>> FactoryMap;
		private readonly IStatusMuter Muter;
	}
}