using System.Collections.Generic;
using System.Diagnostics;
using Twice.ViewModels.Twitter;

namespace Twice.ViewModels.Columns
{
	internal abstract class ColumnViewModelBase : ViewModelBaseEx, IColumnViewModel
	{
		protected ColumnViewModelBase()
		{
			Width = 300;
		}

		public abstract Icon Icon { get; }
		public abstract ICollection<StatusViewModel> Statuses { get; }
		public abstract string Title { get; }

		public double Width
		{
			[DebuggerStepThrough]
			get
			{
				return _Width;
			}
			set
			{
				// ReSharper disable once CompareOfFloatsByEqualityOperator
				if( _Width == value )
				{
					return;
				}

				_Width = value;
				RaisePropertyChanged();
			}
		}

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private double _Width;
	}
}