using System.Collections.Generic;
using System.ComponentModel;
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

			AvailableColumnTypes =
				ColumnTypeListFactory.GetItems( types ).Select( t => new ItemSelection<ColumnTypeItem>( t, true ) ).ToList();

			foreach( var c in AvailableColumnTypes )
			{
				c.PropertyChanged += ColumnType_PropertyChanged;
			}

			SelectAll = true;
		}

		private void ColumnType_PropertyChanged( object sender, PropertyChangedEventArgs e )
		{
			if( e.PropertyName != nameof(ItemSelection<ColumnTypeItem>.IsSelected) )
			{
				return;
			}

			UpdateSelectAll();
		}

		private void UpdateSelectAll()
		{
			if( InSelection )
			{
				return;
			}

			bool all = AvailableColumnTypes.All( c => c.IsSelected );
			_SelectAll = all;

			RaisePropertyChanged( nameof(SelectAll) );
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

				InSelection = true;
				_SelectAll = value;
				foreach( var type in AvailableColumnTypes )
				{
					type.IsSelected = _SelectAll;
				}

				InSelection = false;

				RaisePropertyChanged();
			}
		}

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private bool _SelectAll;

		private bool InSelection;
	}
}