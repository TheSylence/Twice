using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using LinqToTwitter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twice.Messages;
using Twice.Models.Cache;
using Twice.Models.Columns;
using Twice.Models.Configuration;
using Twice.Models.Twitter;
using Twice.Models.Twitter.Streaming;
using Twice.ViewModels.Columns;
using Twice.ViewModels.Twitter;

namespace Twice.Tests.ViewModels.Columns
{
	[TestClass, ExcludeFromCodeCoverage]
	public class MessageColumnTests
	{
		[TestMethod, TestCategory( "ViewModels.Columns" )]
		public async Task MessagesAreLoadedFromCacheAndTwitter()
		{
			// Arrange
			var dm1 = DummyGenerator.CreateDummyMessage();
			dm1.ID = 1;
			dm1.Sender = DummyGenerator.CreateDummyUser( 1 );
			dm1.Recipient = DummyGenerator.CreateDummyUser( 2 );
			dm1.SenderID = 1;
			dm1.RecipientID = 2;

			var dm2 = DummyGenerator.CreateDummyMessage();
			dm2.ID = 2;
			dm2.Sender = DummyGenerator.CreateDummyUser( 2 );
			dm2.Recipient = DummyGenerator.CreateDummyUser( 1 );
			dm2.SenderID = 2;
			dm2.RecipientID = 1;

			var dm3 = DummyGenerator.CreateDummyMessage();
			dm3.ID = 3;
			dm3.Sender = DummyGenerator.CreateDummyUser( 1 );
			dm3.Recipient = DummyGenerator.CreateDummyUser( 3 );
			dm3.SenderID = 1;
			dm3.RecipientID = 3;

			var cachedMessages = new List<MessageCacheEntry> {new MessageCacheEntry( dm1 ), new MessageCacheEntry( dm2 )};
			var incomingMessages = new List<DirectMessage> {dm1};
			var outgoingMessages = new List<DirectMessage> {dm2, dm3};

			var cache = new Mock<ICache>();
			cache.Setup( c => c.AddMessages( It.IsAny<IList<MessageCacheEntry>>() ) ).Returns( Task.CompletedTask ).Verifiable();
			cache.Setup( c => c.GetMessages() ).Returns( Task.FromResult( cachedMessages ) );

			var context = new Mock<IContextEntry>();
			context.SetupGet( c => c.UserId ).Returns( 1 );
			context.Setup( c => c.Twitter.Messages.IncomingMessages( It.IsAny<int>(), It.IsAny<ulong?>() ) ).Returns(
				Task.FromResult( incomingMessages ) );
			context.Setup( c => c.Twitter.Messages.OutgoingMessages( It.IsAny<int>(), It.IsAny<ulong?>() ) ).Returns(
				Task.FromResult( outgoingMessages ) );

			var definition = new ColumnDefinition( ColumnType.User );
			var config = new Mock<IConfig>();
			config.SetupGet( c => c.General ).Returns( new GeneralConfig() );
			var parser = new Mock<IStreamParser>();
			var vm = new MessageColumn( context.Object, definition, config.Object, parser.Object )
			{
				Dispatcher = new SyncDispatcher(),
				Cache = cache.Object
			};

			// Act
			await vm.Load();

			// Assert
			cache.Verify( c => c.AddMessages( It.IsAny<IList<MessageCacheEntry>>() ), Times.Once() );
			Assert.AreEqual( 3, vm.Items.Count );

			var partners = vm.Items.OfType<MessageViewModel>().Select( i => i.Partner.UserId ).Distinct().ToArray();
			CollectionAssert.AreEquivalent( new ulong[] {2, 3}, partners );
		}

		[TestMethod, TestCategory( "ViewModels.Columns" )]
		public void ReceivingDmMessageAddsDm()
		{
			// Arrange
			var messenger = new Messenger();

			var dm = DummyGenerator.CreateDummyMessage();

			var context = new Mock<IContextEntry>();
			var definition = new ColumnDefinition( ColumnType.User );
			var config = new Mock<IConfig>();
			config.SetupGet( c => c.General ).Returns( new GeneralConfig() );
			var parser = new Mock<IStreamParser>();
			var vm = new MessageColumn( context.Object, definition, config.Object, parser.Object, messenger )
			{
				Dispatcher = new SyncDispatcher()
			};

			// Act
			messenger.Send( new DmMessage( dm, EntityAction.Create ) );

			// Assert
			Assert.AreEqual( 1, vm.Items.Count );
		}

		[TestMethod, TestCategory( "ViewModels.Columns" )]
		public void StatusIsRejected()
		{
			// Arrange
			var context = new Mock<IContextEntry>();
			var config = new Mock<IConfig>();
			config.SetupGet( c => c.General ).Returns( new GeneralConfig() );
			var parser = new Mock<IStreamParser>();

			var vm = new TestColumn( context.Object, new ColumnDefinition( ColumnType.Messages ), config.Object, parser.Object );

			// Act
			bool suitable = vm.Suitable( DummyGenerator.CreateDummyStatus() );

			// Assert
			Assert.IsFalse( suitable );
		}

		private class TestColumn : MessageColumn
		{
			public TestColumn( IContextEntry context, ColumnDefinition definition, IConfig config, IStreamParser parser,
				IMessenger messenger = null )
				: base( context, definition, config, parser, messenger )
			{
			}

			public bool Suitable( Status status )
			{
				return IsSuitableForColumn( status );
			}
		}
	}
}