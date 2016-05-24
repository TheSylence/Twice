using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Anotar.NLog;
using Fody;
using GalaSoft.MvvmLight.CommandWpf;
using LinqToTwitter;
using Ninject;
using Twice.Models.Cache;
using Twice.Models.Columns;
using Twice.Models.Configuration;
using Twice.Models.Twitter;
using Twice.Models.Twitter.Streaming;
using Twice.Resources;
using Twice.Services.Views;
using Twice.Utilities;
using Twice.Utilities.Ui;
using Twice.ViewModels.Twitter;

namespace Twice.ViewModels.Columns
{
	[ConfigureAwait( false )]
	internal abstract class ColumnViewModelBase : ViewModelBaseEx, IColumnViewModel
	{
		protected ColumnViewModelBase( IContextEntry context, ColumnDefinition definition, IConfig config,
			IStreamParser parser )
		{
			Configuration = config;
			Definition = definition;
			Context = context;
			Width = definition.Width;
			IsLoading = true;
			Statuses = StatusCollection = new SmartCollection<StatusViewModel>();
			Parser = parser;

			ColumnConfiguration = new ColumnConfigurationViewModel( definition );
			ColumnConfiguration.Saved += ColumnConfiguration_Saved;

			if( config.General.RealtimeStreaming )
			{
				Parser.FriendsReceived += Parser_FriendsReceived;
				Parser.StatusReceived += Parser_StatusReceived;
				Parser.StatusDeleted += Parser_StatusDeleted;
			}

			ActionDispatcher = new ColumnActionDispatcher();
			ActionDispatcher.HeaderClicked += ActionDispatcher_HeaderClicked;
			ActionDispatcher.BottomReached += ActionDispatcher_BottomReached;

			MaxIdFilterExpression = s => s.MaxID == MaxId - 1;
			SinceIdFilterExpression = s => s.SinceID == SinceId;
			SubTitle = "@" + context.AccountName;
		}

		public event EventHandler Changed;

		public event EventHandler Deleted;

		public event EventHandler<StatusEventArgs> NewStatus;

		public async Task Load()
		{
			Parser.StartStreaming();

			await OnLoad().ContinueWith( t =>
			{
				IsLoading = false;
				RaisePropertyChanged( nameof( IsLoading ) );
			} );
		}

		protected async Task AddStatus( StatusViewModel status, bool append = true )
		{
			SinceId = Math.Min( SinceId, status.Id );
			MaxId = Math.Min( MaxId, status.Id );

			await Dispatcher.RunAsync( () =>
			{
				if( append )
				{
					StatusCollection.Add( status );
				}
				else
				{
					StatusCollection.Insert( 0, status );
				}
			} );
			RaiseNewStatus( status );

			await UpdateCache( status.Model );
		}

		protected async Task AddStatuses( IEnumerable<StatusViewModel> statuses, bool append = true )
		{
			var statusViewModels = statuses as StatusViewModel[] ?? statuses.ToArray();
			if( statusViewModels.Any() )
			{
				SinceId = Math.Max( SinceId, statusViewModels.Max( s => s.Id ) );
				MaxId = Math.Min( MaxId, statusViewModels.Min( s => s.Id ) );

				foreach( var s in statusViewModels )
				{
					await UpdateCache( s.Model );

					if( append )
					{
						await Dispatcher.RunAsync( () => StatusCollection.Add( s ) );
					}
					else
					{
						await Dispatcher.RunAsync( () => StatusCollection.Insert( 0, s ) );
					}
				}

				RaiseNewStatus( statusViewModels.Last() );
			}
		}

		protected abstract bool IsSuitableForColumn( Status status );

		protected virtual async Task LoadMoreData()
		{
			var statuses = await Context.Twitter.Statuses.Filter( StatusFilterExpression, MaxIdFilterExpression );
			var list = new List<StatusViewModel>();
			foreach( var s in statuses.Where( s => !Muter.IsMuted( s ) ) )
			{
				list.Add( await CreateViewModel( s ) );
			}

			await AddStatuses( list );
		}

		protected virtual async Task LoadTopData()
		{
			var statuses = await Context.Twitter.Statuses.Filter( StatusFilterExpression, SinceIdFilterExpression );
			var list = new List<StatusViewModel>();
			foreach( var s in statuses.Where( s => !Muter.IsMuted( s ) ).Reverse() )
			{
				list.Add( await CreateViewModel( s ) );
			}

			await AddStatuses( list, false );
		}

		protected virtual async Task OnLoad()
		{
			var statuses = await Context.Twitter.Statuses.Filter( StatusFilterExpression );
			var list = new List<StatusViewModel>();
			foreach( var s in statuses.Where( s => !Muter.IsMuted( s ) ) )
			{
				list.Add( await CreateViewModel( s ) );
			}

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

		private async Task<StatusViewModel> CreateViewModel( Status s )
		{
			var vm = new StatusViewModel( s, Context, Configuration, ViewServiceRepository );

			var quoteId = vm.ExtractQuotedTweetUrl();
			if( quoteId != 0 )
			{
				var quoted = await Context.Twitter.Statuses.GetTweet( quoteId, false );
				if( quoted != null )
				{
					vm.QuotedTweet = await CreateViewModel( quoted );
				}
			}

			return vm;
		}

		private void ExecuteClearCommand()
		{
			Statuses.Clear();
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

		private async void Parser_FriendsReceived( object sender, FriendsStreamEventArgs e )
		{
			var completeList = e.Friends.ToList();
			LogTo.Info( $"Received {completeList.Count} of user's friends" );
			var usersToAdd = new List<User>( completeList.Count );

			while( completeList.Any() )
			{
				var userList = string.Join( ",", completeList.Take( 100 ) );
				completeList.RemoveRange( 0, Math.Min( 100, completeList.Count ) );

				var userData = await Context.Twitter.Users.LookupUsers( userList );
				usersToAdd.AddRange( userData );
			}

			Debug.Assert( usersToAdd.Count == e.Friends.Length );
			await Cache.AddUsers( usersToAdd.Select( u => new UserCacheEntry( u ) ).ToList() );
		}

		private void Parser_StatusDeleted( object sender, DeleteStreamEventArgs e )
		{
			// TODO: Handle deletion
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
			await AddStatus( s, false );
		}

		private async Task UpdateCache( Status status )
		{
			await Cache.AddUsers( status.Entities.UserMentionEntities.Select( m => new UserCacheEntry( m ) ).ToList() );
			await Cache.AddHashtags( status.Entities.HashTagEntities.Select( h => h.Tag ).ToList() );
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

		public IStatusMuter Muter { get; set; }
		public ICollection<StatusViewModel> Statuses { get; }

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

		protected virtual Expression<Func<Status, bool>> MaxIdFilterExpression { get; }

		protected ulong SinceId { get; private set; } = ulong.MinValue;

		protected virtual Expression<Func<Status, bool>> SinceIdFilterExpression { get; }

		protected abstract Expression<Func<Status, bool>> StatusFilterExpression { get; }

		protected readonly IContextEntry Context;

		private readonly IStreamParser Parser;

		private readonly SmartCollection<StatusViewModel> StatusCollection;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private RelayCommand _ClearCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private RelayCommand _DeleteCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private bool _IsLoading;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private string _SubTitle;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private string _Title;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private double _Width;
	}
}