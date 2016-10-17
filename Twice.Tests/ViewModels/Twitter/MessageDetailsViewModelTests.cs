using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Twice.Models.Cache;
using Twice.Models.Twitter;
using Twice.ViewModels.Twitter;

namespace Twice.Tests.ViewModels.Twitter
{
	[TestClass, ExcludeFromCodeCoverage]
	public class MessageDetailsViewModelTests
	{
		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public async Task ConversationIsCorrectlyLoaded()
		{
			// Arrange
			var user1 = DummyGenerator.CreateDummyUser( 1 );
			var user2 = DummyGenerator.CreateDummyUser( 2 );
			var user3 = DummyGenerator.CreateDummyUser( 3 );

			var msg1 = DummyGenerator.CreateDummyMessage( user1, user2, 1 );
			var msg2 = DummyGenerator.CreateDummyMessage( user2, user1, 2 );
			var msg3 = DummyGenerator.CreateDummyMessage( user1, user3, 3 );

			var messageList = new List<MessageCacheEntry>
			{
				new MessageCacheEntry( 1, 1, 2, JsonConvert.SerializeObject( msg1 ) ),
				new MessageCacheEntry( 2, 2, 1, JsonConvert.SerializeObject( msg2 ) ),
				new MessageCacheEntry( 3, 1, 3, JsonConvert.SerializeObject( msg3 ) )
			};

			var context = new Mock<IContextEntry>();
			context.SetupGet( c => c.UserId ).Returns( 1 );

			var cache = new Mock<ICache>();
			cache.Setup( c => c.GetMessages() ).Returns( Task.FromResult( messageList ) );

			var vm = new MessageDetailsViewModel
			{
				Cache = cache.Object,
				Dispatcher = new SyncDispatcher(),
				Message = new MessageViewModel( msg2, context.Object, null, null )
			};

			// Act
			await vm.OnLoad( null );

			// Assert
			Assert.AreEqual( 1, vm.PreviousMessages.Count );
		}
	}
}