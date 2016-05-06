using System;
using System.Collections.Generic;
using System.Linq;
using MaterialDesignThemes.Wpf;
using Twice.Models.Columns;
using Twice.Resources;

namespace Twice.ViewModels.ColumnManagement
{
	internal static class ColumnTypeListFactory
	{
		internal static IEnumerable<ColumnTypeItem> GetItems()
		{
			return GetItems( Enum.GetValues( typeof(ColumnType) ).Cast<ColumnType>() );
		}

		internal static IEnumerable<ColumnTypeItem> GetItems( IEnumerable<ColumnType> types )
		{
			var typeList = types.ToArray();

			if( typeList.Contains( ColumnType.Timeline ) )
			{
				yield return
					new ColumnTypeItem( ColumnType.Timeline, Strings.Timeline, Strings.TimelineDescription, PackIconKind.Home );
			}

			if( typeList.Contains( ColumnType.Mentions ) )
			{
				yield return
					new ColumnTypeItem( ColumnType.Mentions, Strings.Mentions, Strings.MentionsDescription, PackIconKind.At );
			}

			if( typeList.Contains( ColumnType.User ) )
			{
				yield return new ColumnTypeItem( ColumnType.User, Strings.User, Strings.UserDescription, PackIconKind.Account );
			}

			if( typeList.Contains( ColumnType.Messages ) )
			{
				yield return
					new ColumnTypeItem( ColumnType.Messages, Strings.Messages, Strings.MessagesDescription, PackIconKind.Message );
			}

			if( typeList.Contains( ColumnType.Favorites ) )
			{
				yield return
					new ColumnTypeItem( ColumnType.Favorites, Strings.Favourites, Strings.FavouritesDescription, PackIconKind.Star );
			}
		}
	}
}