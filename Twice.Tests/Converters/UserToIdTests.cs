using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using Twice.Converters;

namespace Twice.Tests.Converters
{
	[TestClass, ExcludeFromCodeCoverage]
	public class UserToIdTests
	{
		[TestMethod, TestCategory( "Converters" )]
		public void ConvertBackThrowsException()
		{
			// Arrange
			var conv = new UserToId();

			// Act
			var ex = ExceptionAssert.Catch<NotSupportedException>( () => conv.ConvertBack( null, null, null, null ) );

			// Assert
			Assert.IsNotNull( ex );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void ConvertingNonUserObjectReturnsUnsetValue()
		{
			// Arrange
			var conv = new UserToId();

			// Act
			var value = conv.Convert( string.Empty, null, null, null );

			// Assert
			Assert.AreEqual( DependencyProperty.UnsetValue, value );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void CorrectIdIsExtracted()
		{
			// Arrange
			var conv = new UserToId();
			var user = DummyGenerator.CreateDummyUser();
			user.UserID = 123;

			// Act
			var id = conv.Convert( user, null, null, null );

			// Assert
			Assert.AreEqual( user.UserID, id );
		}
	}
}