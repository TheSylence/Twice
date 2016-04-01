using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Threading;
using LinqToTwitter;
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
			ActionDispatcher = new ColumnActionDispatcher();

			ActionDispatcher.HeaderClicked += ActionDispatcher_HeaderClicked;
			ActionDispatcher.BottomReached += ActionDispatcher_BottomReached;

			MaxIdFilterExpression = s => s.MaxID == MaxId - 1;
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
			SinceId = Math.Min( SinceId, status.Id );
			MaxId = Math.Min( MaxId, status.Id );

			await DispatcherHelper.RunAsync( () => StatusCollection.Add( status ) );
			RaiseNewStatus( status );
		}

		protected async Task AddStatuses( IEnumerable<StatusViewModel> statuses )
		{
			var statusViewModels = statuses as StatusViewModel[] ?? statuses.ToArray();
			if( statusViewModels.Any() )
			{
				SinceId = Math.Max( SinceId, statusViewModels.Max( s => s.Id ) );
				MaxId = Math.Min( MaxId, statusViewModels.Min( s => s.Id ) );

				await DispatcherHelper.RunAsync( () => StatusCollection.AddRange( statusViewModels ) );
				RaiseNewStatus( statusViewModels.Last() );
			}
		}

		protected virtual async Task LoadMoreData()
		{
			var query = Context.Twitter.Status.Where( StatusFilterExpression );
			query = query.Where( MaxIdFilterExpression );

			var statuses = await query.ToListAsync();
			var list = statuses.Where( s => !Muter.IsMuted( s ) ).Select( s => new StatusViewModel( s, Context ) );

			await AddStatuses( list );
		}

		protected virtual async Task OnLoad()
		{
			var statuses = await Context.Twitter.Status.Where( StatusFilterExpression ).ToListAsync();
			var list = statuses.Where( s => !Muter.IsMuted( s ) ).Select( s => new StatusViewModel( s, Context ) );

			await AddStatuses( list );
		}

		protected void RaiseNewStatus( StatusViewModel status )
		{
			if( !IsLoading )
			{
				NewStatus?.Invoke( this, new StatusEventArgs( status ) );
			}
		}

		private async void ActionDispatcher_BottomReached( object sender, EventArgs e )
		{
			IsLoading = true;
			await Task.Run( async () =>
			{
				await LoadMoreData().ContinueWith( t =>
				{
					IsLoading = false;
				} );
			} );
		}

		private void ActionDispatcher_HeaderClicked( object sender, EventArgs e )
		{
			if( !Configuration.General.RealtimeStreaming )
			{
				// TODO: Refresh column
			}
		}

		public IColumnActionDispatcher ActionDispatcher { get; }
		public ColumnDefinition Definition { get; }
		public abstract Icon Icon { get; }

		public bool IsLoading
		{
			[DebuggerStepThrough]
			get
			{
				return _IsLoading;
			}
			private set
			{
				if( _IsLoading == value )
				{
					return;
				}

				_IsLoading = value;
				RaisePropertyChanged();
			}
		}

		public IStatusMuter Muter { get; set; }
		public ICollection<StatusViewModel> Statuses { get; }

		public string Title
		{
			[DebuggerStepThrough]
			get { return _Title; }
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
			[DebuggerStepThrough]
			get { return _Width; }
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

		protected ulong MaxId { get; private set; } = ulong.MaxValue;
		protected virtual Expression<Func<Status, bool>> MaxIdFilterExpression { get; }
		protected ulong SinceId { get; private set; } = ulong.MinValue;
		protected abstract Expression<Func<Status, bool>> StatusFilterExpression { get; }
		protected readonly IContextEntry Context;

		private readonly SmartCollection<StatusViewModel> StatusCollection;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private bool _IsLoading;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private string _Title;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private double _Width;
	}
}