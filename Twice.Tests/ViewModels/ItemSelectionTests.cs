using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;
using Twice.ViewModels;

namespace Twice.Tests.ViewModels
{
	[TestClass, ExcludeFromCodeCoverage]
	public class ItemSelectionTests
	{
		[TestMethod, TestCategory( "ViewModels" )]
		public void NotifyPropertyChangedIsImplementedCorrectly()
		{
			// Arrange
			var obj = new ItemSelection<string>( "test" );
			var tester = new PropertyChangedTester( obj );

			// Act
			tester.Test();

			// Assert
			tester.Verify();
		}
	}
}