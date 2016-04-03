using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Twice.Models.Columns;

namespace Twice.ViewModels.ColumnManagement
{
	internal class ColumnTypeSelectionDialogViewModel : DialogViewModel, IColumnTypeSelectionDialogViewModel
	{
		public ColumnTypeSelectionDialogViewModel()
		{
			var types = new[] {ColumnType.Mentions, ColumnType.Timeline, ColumnType.Messages};

			AvailableColumnTypes = ColumnTypeListFactory.GetItems( types ).Select( t => new ItemSelection<ColumnTypeItem>( t, true ) ).ToList();
		}

		public ICollection<ItemSelection<ColumnTypeItem>> AvailableColumnTypes { get; }

		public bool SelectAll
		{
			[DebuggerStepThrough] get { return _SelectAll; }
			set
			{
				if( _SelectAll == value )
				{
					return;
				}

				_SelectAll = value;
				foreach( var type in AvailableColumnTypes )
				{
					type.IsSelected = _SelectAll;
				}
				RaisePropertyChanged();
			}
		}

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private bool _SelectAll;
	}
}