using GongSolutions.Wpf.DragDrop;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Twice.Models.Columns;
using Twice.ViewModels.Columns;

namespace Twice.ViewModels.Main
{
	[ExcludeFromCodeCoverage]
	internal class DropHandler : DefaultDropHandler
	{
		public DropHandler( IColumnDefinitionList columnList )
		{
			ColumnList = columnList;
		}

		public override void Drop( IDropInfo dropInfo )
		{
			base.Drop( dropInfo );

			if( !dropInfo.IsSameDragDropContextAsSource )
			{
				return;
			}

			var columns = dropInfo.TargetCollection as IEnumerable<IColumnViewModel>;
			Debug.Assert( columns != null );
			ColumnList.Save( columns.Select( c => c.Definition ) );
		}

		private readonly IColumnDefinitionList ColumnList;
	}
}