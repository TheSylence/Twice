using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Twice.Models.Twitter;
using Twice.ViewModels.Twitter;
using Twice.Views.Services;

namespace Twice.Tests.ViewModels.Twitter
{
	[TestClass, ExcludeFromCodeCoverage]
	public class MessageViewModelTests
	{
		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void ReplyCommandOpensDialog()
		{
			// Arrange
			var viewServices = new Mock<IViewServiceRepository>();
			viewServices.Setup( v => v.ReplyToMessage( It.Is<MessageViewModel>( m => m.Id == 123 ) ) ).Returns( Task.CompletedTask ).Verifiable();

			var context = new Mock<IContextEntry>();

			var msg = DummyGenerator.CreateDummyMessage();
			msg.ID = 123;
			var vm = new MessageViewModel( msg, context.Object, null, viewServices.Object );

			// Act
			vm.ReplyCommand.Execute( null );

			// Assert
			viewServices.Verify( v => v.ReplyToMessage( It.Is<MessageViewModel>( m => m.Id == 123 ) ), Times.Once() );
		}
	}
}