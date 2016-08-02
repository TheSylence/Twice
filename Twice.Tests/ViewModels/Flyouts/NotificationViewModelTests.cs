using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;
using Twice.ViewModels;
using Twice.ViewModels.Flyouts;

namespace Twice.Tests.ViewModels.Flyouts
{
	[TestClass, ExcludeFromCodeCoverage]
	public class NotificationViewModelTests
	{
		[TestMethod, TestCategory( "ViewModels.Flyouts" )]
		public void PropertyChangedIsImplementedCorrectly()
		{
			// Arrange
			var vm = new NotificationViewModel( "", NotificationType.Information, true );
			var tester = new PropertyChangedTester( vm );

			// Act
			tester.Test();

			// Assert
			tester.Verify();
		}
	}
}