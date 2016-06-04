using MaterialDesignThemes.Wpf;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;
using Twice.Converters;

namespace Twice.Tests.Converters
{
	[TestClass, ExcludeFromCodeCoverage]
	public class IconToVisualTests
	{
		[TestMethod, TestCategory( "Converters" )]
		public void ConvertBackThrowsException()
		{
			// Arrange
			var conv = new IconToVisual();

			// Act
			var ex = ExceptionAssert.Catch<NotSupportedException>( () => conv.ConvertBack( null, null, null, null ) );

			// Assert
			Assert.IsNotNull( ex );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void CorrectIconIsReturned()
		{
			// Arrange
			var conv = new IconToVisual();

			// Act
			var icon = conv.Convert( "Home", null, null, null );

			// Assert
			Assert.AreEqual( PackIconKind.Home, icon );
		}
	}
}