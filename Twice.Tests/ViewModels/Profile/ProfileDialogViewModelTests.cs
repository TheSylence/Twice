using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twice.ViewModels.Profile;
using Twice.ViewModels.Twitter;

namespace Twice.Tests.ViewModels.Profile
{
	[TestClass]
	public class ProfileDialogViewModelTests
	{
		[TestMethod, TestCategory( "ViewModels.Profile" )]
		public void NotifyPropertyChangedIsImplementedCorrectly()
		{
			// Arrange
			var typeResolver = new Mock<ITypeResolver>();
			typeResolver.Setup( t => t.Resolve( typeof( UserViewModel ) ) ).Returns( new UserViewModel( DummyGenerator.CreateDummyUser() ) );

			var vm = new ProfileDialogViewModel();
			var tester = new PropertyChangedTester( vm, false, typeResolver.Object );

			// Act
			tester.Test();

			// Assert
			tester.Verify();
		}
	}
}