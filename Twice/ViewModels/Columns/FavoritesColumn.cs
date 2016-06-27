using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using LinqToTwitter;
using Seal.Fody;
using Twice.Models.Columns;
using Twice.Models.Configuration;
using Twice.Models.Twitter;
using Twice.Models.Twitter.Streaming;
using Twice.ViewModels.Twitter;

namespace Twice.ViewModels.Columns
{
	[LeaveUnsealed]
	internal class FavoritesColumn : ColumnViewModelBase
	{
		public FavoritesColumn( IContextEntry context, ColumnDefinition definition, IConfig config, IStreamParser parser,
			IMessenger messenger = null )
			: base( context, definition, config, parser, messenger )
		{
			MaxIdFilterExpressionFavorites = s => s.MaxID == MaxId - 1;
			SinceIdFilterExpressionFavorites = s => s.SinceID == SinceId;
		}

		protected override bool IsSuitableForColumn( Status status )
		{
			return status.Favorited;
		}

		protected override bool IsSuitableForColumn( DirectMessage message )
		{
			return false;
		}

		protected override async Task LoadMoreData()
		{
			var statuses = await Context.Twitter.Favorites.List( Context.UserId, MaxIdFilterExpressionFavorites );
			var list = new List<StatusViewModel>();
			foreach( var s in statuses )
			{
				list.Add( await CreateViewModel( s ) );
			}

			await AddItems( list );
		}

		protected override async Task LoadTopData()
		{
			var statuses =
				await Context.Twitter.Favorites.List( Context.UserId, SinceIdFilterExpressionFavorites );
			var list = new List<StatusViewModel>();
			foreach( var s in statuses.Where( s => !Muter.IsMuted( s ) ).Reverse() )
			{
				list.Add( await CreateViewModel( s ) );
			}

			await AddItems( list );
		}

		protected override async Task OnFavorite( Status status )
		{
			var vm = await CreateViewModel( status );
			await AddItem( vm );
		}

		protected override async Task OnLoad()
		{
			var statuses = await Context.Twitter.Favorites.List( Context.UserId );
			var list = new List<StatusViewModel>();
			foreach( var s in statuses )
			{
				list.Add( await CreateViewModel( s ) );
			}

			await AddItems( list );
		}

		protected override async Task OnUnfavorite( Status status )
		{
			await RemoveItem( status.GetStatusId() );
		}

		public override Icon Icon => Icon.Favorites;
		protected override Expression<Func<Status, bool>> StatusFilterExpression => s => s.Favorited;
		private Expression<Func<Favorites, bool>> MaxIdFilterExpressionFavorites { get; }
		private Expression<Func<Favorites, bool>> SinceIdFilterExpressionFavorites { get; }
	}
}