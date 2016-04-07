using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twice.Resources;
using Twice.ViewModels.Validation;

namespace Twice.Tests.ViewModels.Validation
{
	[TestClass]
	public class CommonValidationsTests
	{
		[TestMethod, TestCategory( "ViewModels.Validation" )]
		public void NotEmptyChecksForNull()
		{
			// Arrange
			var setup = new ValidationSetup<int?>();

			// Act
			setup.NotEmpty();

			// Assert
			Assert.IsTrue( setup.CheckFunc( 1 ) );
			Assert.IsFalse( setup.CheckFunc( null ) );
		}

		[TestMethod, TestCategory( "ViewModels.Validation" )]
		public void NotEmptyRespectsWhitespaces()
		{
			// Arrange
			var emptyString = "\t \r\n";
			var whitespaceSetup = new ValidationSetup<string>();
			var setup = new ValidationSetup<string>();

			// Act
			whitespaceSetup.NotEmpty( true );
			setup.NotEmpty();

			// Assert
			Assert.IsTrue( whitespaceSetup.CheckFunc( emptyString ) );
			Assert.IsFalse( setup.CheckFunc( emptyString ) );
		}

		[TestMethod, TestCategory( "ViewModels.Validation" )]
		public void NotEmptySetsCorrectMessage()
		{
			// Arrange
			var setup = new ValidationSetup<string>();

			// Act
			setup.NotEmpty();

			// Assert
			Assert.AreEqual( Strings.ValueMustNotBeEmpty, setup.MessageString );
		}

		[TestMethod, TestCategory( "ViewModels.Validation" )]
		public void SelectedNotNullChecksForNull()
		{
			// Arrange
			var setup = new ValidationSetup<object>();

			// Act
			setup.SelectedNotNull();

			// Assert
			Assert.IsTrue( setup.CheckFunc( new object() ) );
			Assert.IsFalse( setup.CheckFunc( null ) );
		}

		[TestMethod, TestCategory( "ViewMOdels.Validation" )]
		public void UniqueConstrainIsChecked()
		{
			// Arrange
			var setup = new ValidationSetup<string>();
			var list = new[] {"test", "abc"};

			// Act
			setup.Unique( list );

			// Assert
			Assert.IsTrue( setup.CheckFunc( "42" ) );
			Assert.IsFalse( setup.CheckFunc( "test" ) );
		}

		[TestMethod, TestCategory( "ViewModels.Validation" )]
		public void UniqueConstrainSetsCorrectMessage()
		{
			// Arrange
			var setup = new ValidationSetup<string>();

			// Act
			setup.Unique( new[] {"a"} );

			// Assert
			Assert.AreEqual( Strings.NameAlreadyExists, setup.MessageString );
		}

		private class ValidationSetup<TValue> : IValidationSetup<TValue>
		{
			public IValidationSetup<TValue> Check( Func<TValue, bool> action )
			{
				CheckFunc = action;
				return this;
			}

			public IValidationSetup<TValue> If( Func<bool> check )
			{
				IfCheck = check;
				return this;
			}

			public void Message( string message )
			{
				MessageString = message;
			}

			public Func<TValue, bool> CheckFunc { get; private set; }
			public Func<bool> IfCheck { get; private set; }
			public string MessageString { get; private set; }
		}
	}
}