using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Twice.Models.Twitter;
using Twice.ViewModels.Twitter;

namespace Twice.ViewModels.Columns
{
	internal abstract class ColumnViewModelBase : ViewModelBaseEx, IColumnViewModel
	{
		protected ColumnViewModelBase( IContextEntry context )
		{
			Context = context;
			Width = 300;
			IsLoading = true;
		}

		public async Task Load()
		{
			await OnLoad();

			IsLoading = false;
			RaisePropertyChanged( nameof( IsLoading ) );
		}

		protected abstract Task OnLoad();

		public abstract Icon Icon { get; }
		public bool IsLoading { get; private set; }
		public abstract ICollection<StatusViewModel> Statuses { get; }
		public abstract string Title { get; protected set; }

		public double Width
		{
			[DebuggerStepThrough] get { return _Width; }
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

		protected readonly IContextEntry Context;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private double _Width;
	}
}