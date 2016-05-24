using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twice.ViewModels.Twitter;

namespace Twice.Tests.ViewModels.Twitter
{
	[TestClass, ExcludeFromCodeCoverage]
	public class TweetDetailsViewModelTests
	{
		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void NotifyPropertyChangedIsImplementedCorrectly()
		{
			// Arrange
			var typeResolver = new Mock<ITypeResolver>();
			typeResolver.Setup( t => t.Resolve( typeof( StatusViewModel ) ) ).Returns( new StatusViewModel( DummyGenerator.CreateDummyStatus(), null, null, null ) );

			var vm = new TweetDetailsViewModel();
			var tester = new PropertyChangedTester( vm, false, typeResolver.Object );

			// Act
			tester.Test( nameof( TweetDetailsViewModel.Context ) );

			// Assert
			tester.Verify();
		}
	}
}