using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Fody;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using LinqToTwitter;
using Ninject;
using Twice.Models.Cache;
using Twice.Models.Columns;
using Twice.Models.Configuration;
using Twice.Models.Twitter;
using Twice.Models.Twitter.Streaming;
using Twice.Resources;
using Twice.Utilities;
using Twice.Utilities.Ui;
using Twice.ViewModels.Twitter;
using Twice.Views.Services;

namespace Twice.ViewModels.Columns
{
	[ConfigureAwait( false )]
	internal abstract class ColumnViewModelBase : ViewModelBaseEx, IColumnViewModel
	{
		protected ColumnViewModelBase( IContextEntry context, ColumnDefinition definition, IConfig config,
			IStreamParser parser, IMessenger messenger = null )
			: base( messenger )
		{
			Configuration = config;
			Definition = definition;
			Context = context;
			Width = definition.Width;
			IsLoading = true;
			Items = ItemCollection = new SmartCollection<ColumnItem>();
			Parser = parser;

			ColumnConfiguration = new ColumnConfigurationViewModel( definition );
			ColumnConfiguration.Saved += ColumnConfiguration_Saved;

			if( config.General.RealtimeStreaming )
			{
				Parser.StatusReceived += Parser_StatusReceived;
				Parser.StatusDeleted += Parser_ItemDeleted;
				Parser.DirectMessageDeleted += Parser_ItemDeleted;
				Parser.DirectMessageReceived += Parser_DirectMessageReceived;
				Parser.FavoriteEventReceived += Parser_FavoriteEventReceived;
			}

			ActionDispatcher = new ColumnActionDispatcher();
			ActionDispatcher.HeaderClicked += ActionDispatcher_HeaderClicked;
			ActionDispatcher.BottomReached += ActionDispatcher_BottomReached;

			MaxIdFilterExpression = s => s.MaxID == MaxId - 1;
			SinceIdFilterExpression = s => s.SinceID == SinceId;
			CountExpression = s => s.Count == config.General.TweetFetchCount;
			SubTitle = "@" + Context.AccountName;
		}

		public virtual event EventHandler Changed;

		public virtual event EventHandler Deleted;

		public virtual event EventHandler<ColumnItemEventArgs> NewItem;

		public async Task Load()
		{
			Parser.StartStreaming();

			await OnLoad().ContinueWith( t =>
			{
				IsLoading = false;
				RaisePropertyChanged( nameof( IsLoading ) );
			} );
		}

		public void UpdateRelativeTimes()
		{
			foreach( var it in Items )
			{
				it.UpdateRelativeTime();
			}
		}

		protected async Task AddItem( MessageViewModel message )
		{
			await Dispatcher.RunAsync( () =>
			{
				var toDelete = ItemCollection.OfType<MessageViewModel>()
					.Where( i => i.Partner.UserId == message.Partner.UserId )
					.ToArray();
				foreach( var item in toDelete )
				{
					ItemCollection.Remove( item );
				}

				int index = GetInsertIndex( message );
				if( index >= 0 )
				{
					ItemCollection.Insert( index, message );
				}
			} );

			RaiseNewItem( message );
		}

		protected async Task AddItem( ScheduleItem item )
		{
			await Dispatcher.RunAsync( () =>
			{
				int index = GetInsertIndex( item );
				if( index >= 0 )
				{
					ItemCollection.Insert( index, item );
				}
			} );

			RaiseNewItem( item );
		}

		protected async Task AddItem( StatusViewModel status )
		{
			SinceId = Math.Min( SinceId, status.Id );
			MaxId = Math.Min( MaxId, status.Id );

			int insertIndex = GetInsertIndex( status );
			if( insertIndex >= 0 )
			{
				await Dispatcher.RunAsync( () => { ItemCollection.Insert( insertIndex, status ); } );
				RaiseNewItem( status );

				await UpdateCache( status.Model );
			}
		}

		protected async Task AddItems( IEnumerable<MessageViewModel> messages )
		{
			var messageViewModels = messages as MessageViewModel[] ?? messages.ToArray();
			if( messageViewModels.Any() )
			{
				foreach( var s in messageViewModels )
				{
					int index = GetInsertIndex( s );
					if( index >= 0 )
					{
						await Dispatcher.RunAsync( () => ItemCollection.Insert( index, s ) );
					}
				}

				RaiseNewItem( messageViewModels.Last() );
			}
		}

		protected async Task AddItems( IEnumerable<StatusViewModel> statuses )
		{
			var statusViewModels = statuses as StatusViewModel[] ?? statuses.ToArray();
			if( statusViewModels.Any() )
			{
				SinceId = Math.Max( SinceId, statusViewModels.Max( s => s.Id ) );
				MaxId = Math.Min( MaxId, statusViewModels.Min( s => s.Id ) );

				foreach( var s in statusViewModels )
				{
					await UpdateCache( s.Model );

					int index = GetInsertIndex( s );
					if( index >= 0 )
					{
						await Dispatcher.RunAsync( () => ItemCollection.Insert( index, s ) );
					}
				}

				RaiseNewItem( statusViewModels.Last() );
			}
		}

		protected Task<MessageViewModel> CreateViewModel( DirectMessage m )
		{
			var vm = new MessageViewModel( m, Context, ViewServiceRepository );

			return Task.FromResult( vm );
		}

		protected async Task<StatusViewModel> CreateViewModel( Status s )
		{
			var vm = new StatusViewModel( s, Context, Configuration, ViewServiceRepository );

			await vm.LoadDataAsync();
			return vm;
		}

		protected abstract bool IsSuitableForColumn( Status status );

		protected abstract bool IsSuitableForColumn( DirectMessage message );

		protected virtual async Task LoadMoreData()
		{
			var statuses =
				await Context.Twitter.Statuses.Filter( CountExpression, StatusFilterExpression, MaxIdFilterExpression );
			var list = new List<StatusViewModel>();
			await Cache.MapStatusesToColumn( statuses, Definition.Id );
			foreach( var s in statuses.Where( s => !Muter.IsMuted( s ) ) )
			{
				list.Add( await CreateViewModel( s ) );
			}

			await AddItems( list );
		}

		protected virtual async Task LoadTopData()
		{
			var statuses =
				await Context.Twitter.Statuses.Filter( CountExpression, StatusFilterExpression, SinceIdFilterExpression );
			await Cache.MapStatusesToColumn( statuses, Definition.Id );
			var list = new List<StatusViewModel>();
			foreach( var s in statuses.Where( s => !Muter.IsMuted( s ) ).Reverse() )
			{
				list.Add( await CreateViewModel( s ) );
			}

			await AddItems( list );
		}

		protected virtual Task OnFavorite( Status status )
		{
			return Task.CompletedTask;
		}

		protected virtual async Task OnLoad()
		{
			var list = new List<StatusViewModel>();
			var cachedStatuses = await Cache.GetStatusesForColumn( Definition.Id, Configuration.General.TweetFetchCount );
			foreach( var s in cachedStatuses.Where( s => !Muter.IsMuted( s ) ) )
			{
				list.Add( await CreateViewModel( s ) );
			}

			await AddItems( list );

			var filterList = new List<Expression<Func<Status, bool>>>( 3 )
			{
				CountExpression,
				StatusFilterExpression
			};

			if( SinceId != 0 )
			{
				filterList.Add( SinceIdFilterExpression );
			}

			var statuses = await Context.Twitter.Statuses.Filter( filterList.ToArray() );
			await Cache.MapStatusesToColumn( statuses, Definition.Id );
			list.Clear();

			foreach( var s in statuses.Where( s => !Muter.IsMuted( s ) ) )
			{
				list.Add( await CreateViewModel( s ) );
			}

			await AddItems( list );
		}

		protected virtual Task OnUnfavorite( Status status )
		{
			return Task.CompletedTask;
		}

		protected void RaiseNewItem( ColumnItem item )
		{
			if( !IsLoading )
			{
				NewItem?.Invoke( this, new ColumnItemEventArgs( item ) );
			}
		}

		protected async Task RemoveItem( ulong itemId )
		{
			// Yes the same id shouldn't be there more than once, but this happens sometimes. so
			// delete all statuses that match the id.
			var toDelete = ItemCollection.Where( s => s.Id == itemId ).ToArray();
			foreach( var status in toDelete )
			{
				await Dispatcher.RunAsync( () => ItemCollection.Remove( status ) );
			}

			if( Cache != null )
			{
				await Cache.RemoveStatus( itemId );
			}
		}

		private async void ActionDispatcher_BottomReached( object sender, EventArgs e )
		{
			IsLoading = true;
			await Task.Run( async () => { await LoadMoreData().ContinueWith( t => { IsLoading = false; } ); } );
		}

		private async void ActionDispatcher_HeaderClicked( object sender, EventArgs e )
		{
			if( !Configuration.General.RealtimeStreaming )
			{
				IsLoading = true;
				await Task.Run( async () => { await LoadTopData().ContinueWith( t => { IsLoading = false; } ); } );
			}
		}

		private void ColumnConfiguration_Saved( object sender, EventArgs e )
		{
			Changed?.Invoke( this, EventArgs.Empty );
		}

		private void ExecuteClearCommand()
		{
			Items.Clear();
		}

		private async void ExecuteDeleteCommand()
		{
			var csa = new ConfirmServiceArgs( Strings.ConfirmDeleteColumn );
			if( !await ViewServiceRepository.Confirm( csa ) )
			{
				return;
			}

			Deleted?.Invoke( this, EventArgs.Empty );
		}

		private int GetInsertIndex( ColumnItem item )
		{
			var newId = item.Id;

			// Statuses are ordered descending by id
			for( int i = 0; i < ItemCollection.Count; ++i )
			{
				if( ItemCollection[i].Id == newId )
				{
					return -1;
				}

				if( ItemCollection[i].Id < newId )
				{
					return i;
				}
			}

			return ItemCollection.Count;
		}

		private async void Parser_DirectMessageReceived( object sender, DirectMessageStreamEventArgs e )
		{
			if( !IsSuitableForColumn( e.Message ) )
			{
				return;
			}

			var it = await CreateViewModel( e.Message );
			await AddItem( it );
		}

		private async void Parser_FavoriteEventReceived( object sender, FavoriteStreamEventArgs e )
		{
			if( e.Source.GetUserId() != Context.UserId )
			{
				return;
			}

			switch( e.Event )
			{
			case StreamEventType.Favorite:
				await OnFavorite( e.TargetStatus );
				break;

			case StreamEventType.Unfavorite:
				await OnUnfavorite( e.TargetStatus );
				break;
			}
		}

		private async void Parser_ItemDeleted( object sender, DeleteStreamEventArgs e )
		{
			await RemoveItem( e.Id );
		}

		private async void Parser_StatusReceived( object sender, StatusStreamEventArgs e )
		{
			if( Muter.IsMuted( e.Status ) )
			{
				return;
			}

			if( !IsSuitableForColumn( e.Status ) )
			{
				return;
			}

			var s = await CreateViewModel( e.Status );
			await AddItem( s );
		}

		private async Task UpdateCache( Status status )
		{
			if( Cache == null )
			{
				return;
			}

			await Task.WhenAll
			(
				Cache.AddUsers( status.Entities.UserMentionEntities.Select( m => new UserCacheEntry( m ) ).ToList() ),
				Cache.AddHashtags( status.Entities.HashTagEntities.Select( h => h.Tag ).ToList() ) 
			);
		}

		public IColumnActionDispatcher ActionDispatcher { get; }
		public ICache Cache { get; set; }
		public ICommand ClearCommand => _ClearCommand ?? ( _ClearCommand = new RelayCommand( ExecuteClearCommand ) );

		public IColumnConfigurationViewModel ColumnConfiguration { get; }

		public ColumnDefinition Definition { get; }

		public ICommand DeleteCommand => _DeleteCommand ?? ( _DeleteCommand = new RelayCommand( ExecuteDeleteCommand ) );

		[Inject]
		public IDispatcher Dispatcher { get; set; }

		public abstract Icon Icon { get; }

		public bool IsLoading
		{
			[DebuggerStepThrough] get { return _IsLoading; }
			protected set
			{
				if( _IsLoading == value )
				{
					return;
				}

				_IsLoading = value;
				RaisePropertyChanged();
			}
		}

		public ICollection<ColumnItem> Items { get; }

		public IStatusMuter Muter { get; set; }

		public string SubTitle
		{
			[DebuggerStepThrough] get { return _SubTitle; }
			set
			{
				if( _SubTitle == value )
				{
					return;
				}

				_SubTitle = value;
				RaisePropertyChanged();
			}
		}

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
				Definition.Width = (int)value;
				RaisePropertyChanged();
				Changed?.Invoke( this, EventArgs.Empty );
			}
		}

		protected ulong MaxId { get; private set; } = ulong.MaxValue;
		protected ulong SinceId { get; private set; } = ulong.MinValue;
		protected abstract Expression<Func<Status, bool>> StatusFilterExpression { get; }
		private Expression<Func<Status, bool>> MaxIdFilterExpression { get; }
		private Expression<Func<Status, bool>> SinceIdFilterExpression { get; }
		protected readonly IContextEntry Context;
		protected readonly Expression<Func<Status, bool>> CountExpression;
		private readonly SmartCollection<ColumnItem> ItemCollection;
		private readonly IStreamParser Parser;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private RelayCommand _ClearCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private RelayCommand _DeleteCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private bool _IsLoading;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private string _SubTitle;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private string _Title;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private double _Width;
	}
}