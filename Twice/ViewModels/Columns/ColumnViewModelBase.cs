using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Threading;
using Twice.Models.Twitter;
using Twice.Utilities;
using Twice.ViewModels.Columns.Definitions;
using Twice.ViewModels.Twitter;

namespace Twice.ViewModels.Columns
{
	internal abstract class ColumnViewModelBase : ViewModelBaseEx, IColumnViewModel
	{
		protected ColumnViewModelBase( IContextEntry context, ColumnDefinition definition )
		{
			Definition = definition;
			Context = context;
			Width = 300;
			IsLoading = true;
			Statuses = StatusCollection = new SmartCollection<StatusViewModel>();
		}

		public event EventHandler<StatusEventArgs> NewStatus;

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

		protected async Task AddStatus( StatusViewModel status )
		{
			await DispatcherHelper.RunAsync( () => StatusCollection.Add( status ) );
			RaiseNewStatus( status );
		}

		protected async Task AddStatuses( IEnumerable<StatusViewModel> statuses )
		{
			var statusViewModels = statuses as StatusViewModel[] ?? statuses.ToArray();
			await DispatcherHelper.RunAsync( () => StatusCollection.AddRange( statusViewModels ) );
			RaiseNewStatus( statusViewModels.Last() );
		}

		protected abstract Task OnLoad();

		protected void RaiseNewStatus( StatusViewModel status )
		{
			if( !IsLoading )
			{
				NewStatus?.Invoke( this, new StatusEventArgs( status ) );
			}
		}

		public ColumnDefinition Definition { get; }
		public abstract Icon Icon { get; }
		public bool IsLoading { get; private set; }
		public IStatusMuter Muter { get; set; }
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

		private readonly SmartCollection<StatusViewModel> StatusCollection;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private string _Title;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private double _Width;
	}
}