using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;
using Twice.Converters;

namespace Twice.Tests.Converters
{
	[TestClass, ExcludeFromCodeCoverage]
	public class LocalizeTests
	{
		[TestMethod, TestCategory( "Converters" )]
		public void ConvertBackThrowsException()
		{
			// Arrange
			var conv = new Localize();

			// Act
			var ex = ExceptionAssert.Catch<NotSupportedException>( () => conv.ConvertBack( null, null, null, null ) );

			// Assert
			Assert.IsNotNull( ex );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void CultureIsAppliedDuringFormat()
		{
			// Arrange
			var conv = new Localize();

			// Act
			var de = conv.Convert( 1.2, null, "{0:F1}", CultureInfo.CreateSpecificCulture( "de-DE" ) );
			var en = conv.Convert( 1.2, null, "{0:F1}", CultureInfo.CreateSpecificCulture( "en-US" ) );

			// Assert
			Assert.AreEqual( "1,2", de );
			Assert.AreEqual( "1.2", en );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void NoFormatReturnsUnsetValue()
		{
			// Arrange
			var conv = new Localize();

			// Act
			var value = conv.Convert( "test", null, null, null );

			// Assert
			Assert.AreEqual( DependencyProperty.UnsetValue, value );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void NoValueReturnsUnsetValue()
		{
			// Arrange
			var conv = new Localize();

			// Act
			var value = conv.Convert( null, null, "test", null );

			// Assert
			Assert.AreEqual( DependencyProperty.UnsetValue, value );
		}
	}
}