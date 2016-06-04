using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using Twice.Converters;

namespace Twice.Tests.Converters
{
	[TestClass, ExcludeFromCodeCoverage]
	public class UtcToLocalTests
	{
		[TestMethod, TestCategory( "Converters" )]
		public void ConvertBackReturnsUtcTime()
		{
			// Arrange
			var conv = new UtcToLocal();

			var now = DateTime.Now;

			// Act
			var nowUtc = (DateTime)conv.ConvertBack( now, null, null, null );

			// Assert
			Assert.AreNotEqual( DateTimeKind.Utc, now.Kind );
			Assert.AreEqual( DateTimeKind.Utc, nowUtc.Kind );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void ConvertedValueIsInLocalTime()
		{
			// Arrange
			var conv = new UtcToLocal();
			var nowUtc = DateTime.UtcNow;

			// Act
			var now = (DateTime)conv.Convert( nowUtc, null, null, null );

			// Assert
			Assert.AreNotEqual( DateTimeKind.Local, nowUtc.Kind );
			Assert.AreEqual( DateTimeKind.Local, now.Kind );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void ConvertingBackInvalidTypeReturnsUnsetValue()
		{
			// Arrange
			var conv = new UtcToLocal();

			// Act
			var v = conv.ConvertBack( string.Empty, null, null, null );

			// Assert
			Assert.AreEqual( DependencyProperty.UnsetValue, v );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void ConvertingInvalidTypeReturnsUnsetValue()
		{
			// Arrange
			var conv = new UtcToLocal();

			// Act
			var v = conv.Convert( string.Empty, null, null, null );

			// Assert
			Assert.AreEqual( DependencyProperty.UnsetValue, v );
		}
	}
}