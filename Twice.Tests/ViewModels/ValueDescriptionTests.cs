using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Twice.Attributes;
using Twice.Resources;
using Twice.ViewModels;

namespace Twice.Tests.ViewModels
{
	[TestClass, ExcludeFromCodeCoverage]
	public class ValueDescriptionTests
	{
		[TestMethod, TestCategory( "ViewModels" )]
		public void CorrectNameIsConstructedForEnum()
		{
			// Arrange

			// Act
			var list = ValueDescription<TestEnum>.GetValues( true );

			// Assert
			Assert.AreEqual( Strings.Cancel, list.First().Name );
		}

		[TestMethod, TestCategory( "ViewModels" )]
		[SuppressMessage( "ReSharper", "SuspiciousTypeConversion.Global" )]
		public void DoesNotEqualsOtherType()
		{
			// Arrange
			var a = new ValueDescription<int>( 123, "test" );
			var b = new ValueDescription<byte>( 123, "test" );

			// Act
			var ab = a.Equals( b );
			var ba = a.Equals( b );

			// Assert
			Assert.IsFalse( ab );
			Assert.IsFalse( ba );
		}

		[TestMethod, TestCategory( "ViewModels" )]
		public void EqOperatorChecksEqual()
		{
			// Arrange
			var a = new ValueDescription<int>( 123, "test" );
			var b = new ValueDescription<int>( 123, "test" );

			// Act
			var ab = a == b;
			var ba = b == a;
			var not = a != b;

			// Assert
			Assert.IsTrue( ab );
			Assert.IsTrue( ba );
			Assert.IsFalse( not );
		}

		[TestMethod, TestCategory( "ViewModels" )]
		public void EqOperatorChecksForReference()
		{
			// Arrange
			var a = new ValueDescription<int>( 123, "test" );
			var b = a;

			// Act
			bool eq = a == b;
			bool nEq = a != b;

			// Assert
			Assert.IsTrue( eq );
			Assert.IsFalse( nEq );
		}

		[TestMethod, TestCategory( "ViewModels" )]
		[SuppressMessage( "ReSharper", "ConditionIsAlwaysTrueOrFalse" )]
		public void EqOperatorWorksWithNull()
		{
			// Arrange
			ValueDescription<int> a = null;
			var b = new ValueDescription<int>( 123, "test" );

			// Act
			var ab = a == b;
			var ba = b == a;
			var not = a != b;

			// Assert
			Assert.IsFalse( ab );
			Assert.IsFalse( ba );
			Assert.IsTrue( not );
		}

		[TestMethod, TestCategory( "ViewModels" )]
		public void EqualsUsesValue()
		{
			// Arrange
			var a = new ValueDescription<int>( 123, "test" );
			var b = new ValueDescription<int>( 123, "test" );
			var c = new ValueDescription<int>( 222, "test" );

			// Act
			var ab = a.Equals( b );
			var ba = b.Equals( a );
			var ac = a.Equals( c );
			var ca = c.Equals( a );

			// Assert
			Assert.IsTrue( ab );
			Assert.IsTrue( ba );
			Assert.IsFalse( ac );
			Assert.IsFalse( ca );
		}

		[TestMethod, TestCategory( "ViewModels" )]
		public void GetHashCodeUsesValue()
		{
			// Arrange
			var desc = new ValueDescription<int>( 123, "test" );

			// Act
			var hash = desc.GetHashCode();

			// Assert
			Assert.AreEqual( 123.GetHashCode(), hash );
		}

		[TestMethod, TestCategory( "ViewModels" )]
		public void GetValuesReturnsNothingForNonEnum()
		{
			// Arrange

			// Act
			var values = ValueDescription<int>.GetValues().ToArray();

			// Assert
			Assert.AreEqual( 0, values.Length );
		}

		[TestMethod, TestCategory( "ViewModels" )]
		public void ImplicitConversationWorks()
		{
			// Arrange
			var vd = new ValueDescription<int>( 123, "test" );

			// Act
			int test = vd;

			// Assert
			Assert.AreEqual( 123, test );
		}

		[TestMethod, TestCategory( "ViewModels" )]
		public void PropertiesAreCorrectlySetDuringConstruction()
		{
			// Arrange
			const int value = 123;
			const string desc = "desc";

			// Act
			var vd = new ValueDescription<int>( value, "desc" );

			// Assert
			Assert.AreEqual( value, vd.Value );
			Assert.AreEqual( desc, vd.Name );
		}

		private enum TestEnum
		{
			[LocalizeKey( "Cancel" )]
			Test
		}
	}
}