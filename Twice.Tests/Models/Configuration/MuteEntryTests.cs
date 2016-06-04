using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;
using Twice.Models.Configuration;

namespace Twice.Tests.Models.Configuration
{
	[TestClass, ExcludeFromCodeCoverage]
	public class MuteEntryTests
	{
		[TestMethod, TestCategory( "Models.Configuration" )]
		public void NotifyPropertyChangedIsImplementedCorrectly()
		{
			// Arrange
			var obj = new MuteEntry();
			var tester = new PropertyChangedTester( obj );

			// Act
			tester.Test();

			// Assert
			tester.Verify();
		}
	}
}