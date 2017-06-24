using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Fody;
using GalaSoft.MvvmLight;
using Twice.Utilities.Ui;
using Twice.ViewModels.Columns;

namespace Twice.ViewModels.Profile
{
	[ConfigureAwait( false )]
	internal class UserSubPage : ObservableObject
	{
		public UserSubPage( string title, Func<Task<IEnumerable<object>>> loadAction,
			Func<Task<IEnumerable<object>>> loadMoreAction, int count )
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
			_Items = new ObservableCollection<object>();
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

				foreach( var item in newData )
				{
					await Dispatcher.RunAsync( () => _Items.Add( item ) );
				}
			}
		}

		public IColumnActionDispatcher ActionDispatcher { get; }
		public int Count { get; }
		public IDispatcher Dispatcher { get; set; }

		public bool IsLoading { get; set; }

		public ICollection<object> Items
		{
			get
			{
				if( !ItemsRequested )
				{
					ItemsRequested = true;
					IsLoading = true;

					Task.Run( async () =>
					{
						var toAdd = await LoadAction();
						foreach( var it in toAdd )
						{
							await Dispatcher.RunAsync( () => _Items.Add( it ) );
						}
					} ).ContinueWith( t => { IsLoading = false; } );
				}

				return _Items;
			}
		}

		public string Title { get; }

		private readonly ObservableCollection<object> _Items;

		private readonly Func<Task<IEnumerable<object>>> LoadAction;
		private readonly Func<Task<IEnumerable<object>>> LoadMoreAction;

		private bool ItemsRequested;
	}
}