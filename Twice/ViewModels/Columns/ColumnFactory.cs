using System;
using System.Collections.Generic;
using System.Linq;
using Twice.Models.Twitter;
using Twice.ViewModels.Columns.Definitions;

namespace Twice.ViewModels.Columns
{
	internal class ColumnFactory
	{
		public ColumnFactory( ITwitterContextList contexts )
		{
			Rand = new Random();
			Contexts = contexts;

			FactoryMap = new Dictionary<ColumnType, Func<ColumnDefinition, ColumnViewModelBase>>
			{
				{ColumnType.User, UserColumn},
				{ColumnType.Timeline, TimelineColumn},
				{ColumnType.Mentions, MentionsColumn}
			};
		}

		public ColumnViewModelBase Construct( ColumnDefinition def )
		{
			Func<ColumnDefinition, ColumnViewModelBase> factoryAction;

			if( FactoryMap.TryGetValue( def.Type, out factoryAction ) )
			{
				var column = factoryAction( def );

				column.Width = def.Width;

				return column;
			}

			return null;
		}

		private ColumnViewModelBase MentionsColumn( ColumnDefinition definition )
		{
			var def = (MentionsColumnDefinition)definition;
			return new MentionsColumn( Contexts.Contexts.FirstOrDefault( c => c.UserId == def.AccountIds.First() ) );
		}

		private IContextEntry RandomContext()
		{
			int index = Rand.Next( Contexts.Contexts.Count );
			return Contexts.Contexts.ElementAt( index );
		}

		ColumnViewModelBase TimelineColumn( ColumnDefinition definition )
		{
			var def = (TimelineColumnDefinition)definition;
			return new TimelineColumn( Contexts.Contexts.FirstOrDefault( c => c.UserId == def.AccountIds.First() ) );
		}

		private ColumnViewModelBase UserColumn( ColumnDefinition definition )
		{
			var def = (UserColumnDefintion)definition;
			return new UserColumn( RandomContext(), def.UserId );
		}

		private readonly ITwitterContextList Contexts;
		private readonly Dictionary<ColumnType, Func<ColumnDefinition, ColumnViewModelBase>> FactoryMap;
		private readonly Random Rand;
	}
}