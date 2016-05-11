using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;
using Twice.Models.Twitter;
using Twice.ViewModels.ColumnManagement;
using Twice.ViewModels.Wizards;

namespace Twice.Tests.ViewModels.ColumnManagement
{
	[TestClass]
	public class SourceAccountSelectorPageTests
	{
		[TestMethod, TestCategory( "ViewModels.ColumnManagement" )]
		public void CorrectAccountsAreListed()
		{
			// Arrange
			var c1 = new Mock<IContextEntry>();
			var c2 = new Mock<IContextEntry>();

			c1.SetupGet( c => c.UserId ).Returns( 123 );
			c2.SetupGet( c => c.UserId ).Returns( 456 );

			c1.SetupGet( c => c.ProfileImageUrl ).Returns( new System.Uri( "http://example.com/image.png" ) );
			c2.SetupGet( c => c.ProfileImageUrl ).Returns( new System.Uri( "http://example.com/image.png" ) );

			var contexts = new[]
			{
				c1.Object,
				c2.Object
			};

			var wizard = new Mock<IWizardViewModel>();
			var contextList = new Mock<ITwitterContextList>();
			contextList.SetupGet( c => c.Contexts ).Returns( contexts );

			var vm = new SourceAccountSelectorPage( wizard.Object, contextList.Object );

			// Act
			var userIds = vm.Accounts.Select( c => c.Context.UserId ).ToArray();

			// Assert
			CollectionAssert.AreEquivalent( new ulong[] {123, 456}, userIds );
		}
	}
}