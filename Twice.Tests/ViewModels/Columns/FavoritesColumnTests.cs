using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using LinqToTwitter;
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
			var config = new Mock<IConfig>();
			config.SetupGet( c => c.General ).Returns( new GeneralConfig {RealtimeStreaming = true} );
			var parser = new Mock<IStreamParser>();
			var context = new Mock<IContextEntry>();
			context.SetupGet( c => c.UserId ).Returns( 456 );

			var cache = new Mock<ICache>();
			cache.Setup( c => c.AddUsers( It.IsAny<IList<UserCacheEntry>>() ) ).Returns( Task.CompletedTask );
			cache.Setup( c => c.AddHashtags( It.IsAny<IList<string>>() ) ).Returns( Task.CompletedTask );

			var waitHandle = new ManualResetEventSlim( false );

			var column = new FavoritesTestColumn( context.Object, null, config.Object, parser.Object )
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

		[TestMethod, TestCategory( "ViewModels.Columns" )]
		public void MessageIsAlwaysRejected()
		{
			// Arrange
			var column = new FavoritesTestColumn();

			var msg = DummyGenerator.CreateDummyMessage();

			// Act
			bool suitable = column.Suitable( msg );

			// Assert
			Assert.IsFalse( suitable );
		}

		private class FavoritesTestColumn : FavoritesColumn
		{
			public FavoritesTestColumn( IContextEntry context = null, ColumnDefinition definition = null, IConfig config = null, IStreamParser parser = null )
				: base( context ?? DefaultContext(), definition ?? DefaultDefinition(), config ?? DefaultConfig(), parser ?? DefaultParser() )
			{
			}

			public void SetLoading( bool loading )
			{
				IsLoading = loading;
			}

			public bool Suitable( DirectMessage msg )
			{
				return IsSuitableForColumn( msg );
			}

			private static IConfig DefaultConfig()
			{
				var cfg = new Mock<IConfig>();
				cfg.SetupGet( c => c.General ).Returns( new GeneralConfig() );

				return cfg.Object;
			}

			private static IContextEntry DefaultContext()
			{
				var context = new Mock<IContextEntry>();

				return context.Object;
			}

			private static ColumnDefinition DefaultDefinition()
			{
				return new ColumnDefinition( ColumnType.Favorites );
			}

			private static IStreamParser DefaultParser()
			{
				var parser = new Mock<IStreamParser>();

				return parser.Object;
			}
		}
	}
}