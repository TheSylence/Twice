using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Twice.Models.Twitter;
using Twice.Utilities;
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
			Statuses = StatusCollection = new SmartCollection<StatusViewModel>();
		}

		public async Task Load()
		{
			await Task.Run( async () =>
			{
				await OnLoad().ContinueWith( t =>
				{
					IsLoading = false;
					RaisePropertyChanged( nameof( IsLoading ) );
				} );
			} );
		}

		protected abstract Task OnLoad();

		public abstract Icon Icon { get; }
		public bool IsLoading { get; private set; }
		public ICollection<StatusViewModel> Statuses { get; }

		public string Title
		{
			[DebuggerStepThrough] get { return _Title; }
			set
			{
				if( _Title == value )
				{
					return;
				}

				_Title = value;
				RaisePropertyChanged();
			}
		}

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

		protected readonly SmartCollection<StatusViewModel> StatusCollection;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private string _Title;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private double _Width;
	}
}