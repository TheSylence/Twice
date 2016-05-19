using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twice.Converters;
using Twice.Resources;
using Twice.Utilities;

namespace Twice.Tests.Converters
{
	[TestClass, ExcludeFromCodeCoverage]
	public class RelativeDateTests
	{
		[TestMethod, TestCategory( "Converters" )]
		public void ConvertBackThrosException()
		{
			// Arrange
			var conv = new RelativeDate();

			// Act
			var ex = ExceptionAssert.Catch<NotSupportedException>( () => conv.ConvertBack( null, null, null, null ) );

			// Assert
			Assert.IsNotNull( ex );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void DayDifferenceIsCorrectlyFormatted()
		{
			// Arrange
			var date = new Mock<IDateProvider>();
			date.SetupGet( d => d.Now ).Returns( new DateTime( 1234, 5, 6, 7, 8, 9 ).ToLocalTime() );

			var conv = new RelativeDate {DateProvider = date.Object};

			// Act
			var result = conv.Convert( new DateTime( 1234, 5, 5, 7, 7, 0 ), null, null, null );

			// Assert
			Assert.AreEqual( $"1{Strings.DaysShort}", result );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void HourDifferenceIsCorrectlyFormatted()
		{
			// Arrange
			var date = new Mock<IDateProvider>();
			date.SetupGet( d => d.Now ).Returns( new DateTime( 1234, 5, 6, 7, 8, 9 ).ToLocalTime() );

			var conv = new RelativeDate {DateProvider = date.Object};

			// Act
			var result = conv.Convert( new DateTime( 1234, 5, 6, 6, 7, 0 ), null, null, null );

			// Assert
			Assert.AreEqual( $"1{Strings.HoursShort}", result );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void MinuteDifferenceIsCorrectlyFormatted()
		{
			// Arrange
			var date = new Mock<IDateProvider>();
			date.SetupGet( d => d.Now ).Returns( new DateTime( 1234, 5, 6, 7, 8, 9 ).ToLocalTime() );

			var conv = new RelativeDate {DateProvider = date.Object};

			// Act
			var result = conv.Convert( new DateTime( 1234, 5, 6, 7, 7, 0 ), null, null, null );

			// Assert
			Assert.AreEqual( $"1{Strings.MinutesShort}", result );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void RecentPastIsConvertedToNow()
		{
			// Arrange
			var date = new Mock<IDateProvider>();
			date.SetupGet( d => d.Now ).Returns( new DateTime( 1234, 5, 6, 7, 8, 9 ).ToLocalTime() );

			var conv = new RelativeDate {DateProvider = date.Object};

			// Act
			var result = conv.Convert( new DateTime( 1234, 5, 6, 7, 8, 0 ), null, null, null );

			// Assert
			Assert.AreEqual( Strings.Now, result );
		}
	}
}