using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Threading;
using Twice.Utilities.Ui;
using Twice.ViewModels.Columns;

namespace Twice.ViewModels.Profile
{
	internal class UserSubPage : ObservableObject
	{
		public IDispatcher Dispatcher { get; set; }

		public UserSubPage( string title, Func<Task<IEnumerable<object>>> loadAction, Func<Task<IEnumerable<object>>> loadMoreAction, int count )
			: this( title, loadAction, count )
		{
			LoadMoreAction = loadMoreAction;

			ActionDispatcher = new ColumnActionDispatcher();
			ActionDispatcher.BottomReached += ActionDispatcher_BottomReached;
		}

		public UserSubPage( string title, Func<Task<IEnumerable<object>>> loadAction, int count )
		{
			Title = title;
			Count = count;
			LoadAction = loadAction;
		}

		private async void ActionDispatcher_BottomReached( object sender, EventArgs e )
		{
			IsLoading = true;
			await Task.Run( async () => { await LoadMoreData().ContinueWith( t => { IsLoading = false; } ); } );
		}

		private async Task LoadMoreData()
		{
			if( LoadMoreAction != null && _Items != null )
			{
				var newData = await LoadMoreAction();

				await Dispatcher.RunAsync( () =>
				{
					foreach( var item in newData )
					{
						_Items.Add( item );
					}
				} );
			}
		}

		public IColumnActionDispatcher ActionDispatcher { get; }
		public int Count { get; }

		public bool IsLoading
		{
			[DebuggerStepThrough] get { return _IsLoading; }

			set
			{
				if( _IsLoading == value )
				{
					return;
				}

				_IsLoading = value;
				RaisePropertyChanged( nameof( IsLoading ) );
			}
		}

		public ICollection<object> Items
		{
			get
			{
				if( _Items == null )
				{
					IsLoading = true;
					Task.Run( async () =>
					{
						_Items = new ObservableCollection<object>( await LoadAction() );
						RaisePropertyChanged( nameof( Items ) );
					} ).ContinueWith( t => { IsLoading = false; } );
				}

				return _Items;
			}
		}

		public string Title { get; }

		private readonly Func<Task<IEnumerable<object>>> LoadAction;
		private readonly Func<Task<IEnumerable<object>>> LoadMoreAction;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private bool _IsLoading;

		private ObservableCollection<object> _Items;
	}
}