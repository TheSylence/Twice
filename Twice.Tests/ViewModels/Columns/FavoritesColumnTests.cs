using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twice.Models.Cache;
using Twice.Models.Columns;
using Twice.Models.Configuration;
using Twice.Models.Twitter;
using Twice.Models.Twitter.Streaming;
using Twice.ViewModels.Columns;

namespace Twice.Tests.ViewModels.Columns
{
	[TestClass, ExcludeFromCodeCoverage]
	public class FavoritesColumnTests
	{
		[TestMethod, TestCategory( "ViewModels.Columns" )]
		public void FavoriteIsAddedToColumn()
		{
			// Arrange
			var definition = new ColumnDefinition( ColumnType.Favorites );
			var config = new Mock<IConfig>();
			config.SetupGet( c => c.General ).Returns( new GeneralConfig {RealtimeStreaming = true} );
			var parser = new Mock<IStreamParser>();
			var context = new Mock<IContextEntry>();
			context.SetupGet( c => c.UserId ).Returns( 456 );

			var cache = new Mock<ICache>();
			cache.Setup( c => c.AddUsers( It.IsAny<IList<UserCacheEntry>>() ) ).Returns( Task.CompletedTask );
			cache.Setup( c => c.AddHashtags( It.IsAny<IList<string>>() ) ).Returns( Task.CompletedTask );

			var waitHandle = new ManualResetEventSlim( false );

			var column = new FavoritesTestColumn( context.Object, definition, config.Object, parser.Object )
			{
				Dispatcher = new SyncDispatcher(),
				Cache = cache.Object
			};
			column.SetLoading( false );
			column.NewItem += ( s, e ) => { waitHandle.Set(); };

			const string json =
				"{\"event\":\"favorite\",\"created_at\":\"Sat Sep 04 16:10:54 +0000 2010\",\"target\":{\"id\": 123},\"source\":{\"id\": 456},\"target_object\":{\"text\": \"Hello World\"}}";

			// Act
			parser.Raise( p => p.FavoriteEventReceived += null, new FavoriteStreamEventArgs( json ) );
			bool set = waitHandle.Wait( 1000 );

			// Assert
			Assert.IsTrue( set, "Event was not fired" );
			Assert.AreEqual( 1, column.Items.Count );
		}

		private class FavoritesTestColumn : FavoritesColumn
		{
			public FavoritesTestColumn( IContextEntry context, ColumnDefinition definition, IConfig config, IStreamParser parser, IMessenger messenger = null )
				: base( context, definition, config, parser, messenger )
			{
			}

			public void SetLoading( bool loading )
			{
				IsLoading = loading;
			}
		}
	}
}