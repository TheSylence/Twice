using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twice.Converters;

namespace Twice.Tests.Converters
{
	[TestClass, ExcludeFromCodeCoverage]
	public class ConverterChainTests
	{
		[TestMethod, TestCategory( "Converters" )]
		public void ConvertBackThrowsException()
		{
			// Arrange
			var conv = new ConverterChain();

			// Act
			var ex = ExceptionAssert.Catch<NotSupportedException>( () => conv.ConvertBack( null, null, null, null ) );

			// Assert
			Assert.IsNotNull( ex );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void ConvertersAreChainedCorrectly()
		{
			// Arrange
			var c1 = new Mock<IValueConverter>();
			var c2 = new Mock<IValueConverter>();
			var c3 = new Mock<IValueConverter>();

			c1.Setup( c => c.Convert( 123, typeof( string ), 444, CultureInfo.InvariantCulture ) ).Returns( 124 ).Verifiable();
			c2.Setup( c => c.Convert( 124, typeof( string ), 444, CultureInfo.InvariantCulture ) ).Returns( 125 ).Verifiable();
			c3.Setup( c => c.Convert( 125, typeof( string ), 444, CultureInfo.InvariantCulture ) ).Returns( 126 ).Verifiable();

			// Act
			var chain = new ConverterChain {c1.Object, c2.Object, c3.Object};

			var result = chain.Convert( 123, typeof( string ), 444, CultureInfo.InvariantCulture );

			// Assert
			Assert.AreEqual( 126, result );
			c1.VerifyAll();
			c2.VerifyAll();
			c3.VerifyAll();
		}
	}
}