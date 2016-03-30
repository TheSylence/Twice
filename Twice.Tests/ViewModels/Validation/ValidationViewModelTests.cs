using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twice.ViewModels.Validation;

namespace Twice.Tests.ViewModels.Validation
{
	[TestClass]
	public class ValidationViewModelTests
	{
		[TestMethod, TestCategory( "ViewModels.Validation" )]
		public void ErrorIsCorrectlyConstructed()
		{
			// Arrange
			var vm = new TestValidation();

			vm.ValidateWrapper( () => vm.Name ).Check( name => name != "test" ).Message( "firstCheck" );
			vm.ValidateWrapper( () => vm.Name ).Check( name => !string.IsNullOrEmpty( name ) ).Message( "secondCheck" );

			// Act
			vm.Name = "test";
			vm.Name = null;
			string before = vm.AllErrors;

			vm.Name = "test";
			string after = vm.AllErrors;

			vm.Name = "123";
			string last = vm.AllErrors;

			// Assert
			Assert.AreEqual( "secondCheck", before );
			Assert.AreEqual( "firstCheck", after );
			Assert.IsNull( last, last );
		}

		[TestMethod, TestCategory( "ViewModels.Validation" )]
		public void ErrorsCanBeCleared()
		{
			// Arrange
			var vm = new TestValidation();

			vm.ValidateWrapper( () => vm.Name ).Check( name => name != "test" ).Message( "check" );

			// Act
			vm.Name = "test";
			vm.Name = null;
			vm.ClearValidationErrors();

			string after = vm.AllErrors;

			// Assert
			Assert.IsNull( after );
		}

		[TestMethod, TestCategory( "ViewModels.Validation" )]
		public void ErrorWithMultipleLinesIsCorrectlyConstructed()
		{
			// Arrange
			var vm = new TestValidation();

			vm.ValidateWrapper( () => vm.Name ).Check( name => !string.IsNullOrEmpty( name ) ).Message( "firstCheck" );
			vm.ValidateWrapper( () => vm.Name ).Check( name => !string.IsNullOrEmpty( name ) ).Message( "secondCheck" );

			// Act
			vm.Name = "test";
			vm.Name = null;
			string error = vm.AllErrors;

			// Assert
			Assert.AreEqual( "firstCheck" + Environment.NewLine + "secondCheck", error );
		}

		[TestMethod, TestCategory( "ViewModels.Validation" )]
		public void ExceptionInValidationActionIsSwallowed()
		{
			// Arrange
			var vm = new TestValidation();

			// Act
			vm.ValidateWrapper( () => vm.Name ).Check( name => { throw new Exception( "test error" ); } ).Message( "test message" );

			vm.Name = "test";

			// Assert
			Assert.AreEqual( "test error", vm.GetErrors( nameof( vm.Name ) ).Cast<string>().First() );
		}

		[TestMethod, TestCategory( "ViewModels.Validation" )]
		public void HasErrorIsCorrectlySet()
		{
			// Arrange
			var vm = new TestValidation();

			vm.ValidateWrapper( () => vm.Name ).Check( name => name != "test" ).Message( "firstCheck" );
			vm.ValidateWrapper( () => vm.Name ).Check( name => !string.IsNullOrEmpty( name ) ).Message( "secondCheck" );

			// Act
			vm.Name = "test";
			vm.Name = null;
			bool before = vm.HasErrors;

			vm.Name = "test";
			bool after = vm.HasErrors;

			vm.Name = "123";
			bool last = vm.HasErrors;

			// Assert
			Assert.IsTrue( before );
			Assert.IsTrue( after );
			Assert.IsFalse( last );
		}

		[TestMethod, TestCategory( "ViewModels.Validation" )]
		public void ManualValidateThrowsWithNullExpression()
		{
			// Arrange
			var vm = new TestValidation();

			// Act
			var ex = ExceptionAssert.Catch<ArgumentNullException>( () => vm.ManualValidate<string>( null ) );

			// Assert
			Assert.IsNotNull( ex );
		}

		[TestMethod, TestCategory( "ViewModels.Validation" )]
		public void ManualValidationIsCheckedOnValidate()
		{
			// Arrange
			var vm = new TestValidation();

			vm.ManualValidate( () => vm.Name ).Check( str => false ).Message( "test" );

			// Act
			vm.ValidateAllWrapper();

			// Assert
			Assert.IsTrue( vm.HasErrors );
		}

		[TestMethod, TestCategory( "ViewModels.Validation" )]
		public void ManualValidationIsNotCheckedOnPropertyChange()
		{
			// Arrange
			var vm = new TestValidation();

			vm.ManualValidate( () => vm.Name ).Check( str => false ).Message( "test" );

			// Act
			vm.Name = "test";

			// Assert
			Assert.IsFalse( vm.HasErrors );
		}

		[TestMethod, TestCategory( "ViewModels.Validation" )]
		public void PropertyWithoutValidationIsNotValidated()
		{
			// Arrange
			var vm = new TestValidation
			{
				// Act
				Name = "test"
			};

			// Assert
			Assert.IsFalse( vm.GetErrors( nameof( vm.Name ) ).Cast<string>().Any() );
		}

		[TestMethod, TestCategory( "ViewModels.Validation" )]
		public void UnchangedPropertyIsNotValidated()
		{
			// Arrange
			var vm = new TestValidation();

			// Act
			vm.ValidateWrapper( () => vm.Name ).Check( name => { throw new Exception( "must not be called" ); } ).Message( "must not be visible" );

			// Assert
			var errors = vm.GetErrors( nameof( vm.Name ) );

			Assert.IsFalse( errors.Cast<string>().Any() );
		}

		[TestMethod, TestCategory( "ViewModels.Validation" )]
		public void ValidateThrowsArgumentExceptionWithEmptyPropertyName()
		{
			// Arrange
			var vm = new TestValidation();

			// Act
			Action action = () => vm.ValidateWrapper<object>( null );

			// Assert
			ExceptionAssert.Throws<ArgumentException>( action );
		}

		[TestMethod, TestCategory( "ViewModels.Validation" )]
		public void ValidationConditionIsCheckedBeforeValidation()
		{
			// Arrange
			var vm = new TestValidation();

			bool precheck = false;

			// ReSharper disable once AccessToModifiedClosure
			vm.ValidateWrapper( () => vm.Name ).Check( name => !string.IsNullOrEmpty( name ) ).If( () => precheck ).Message( "test" );

			// Act
			vm.ValidateAllWrapper();
			bool unmetCondition = vm.HasErrors;

			precheck = true;
			vm.ValidateAllWrapper();
			bool metCondition = vm.HasErrors;

			// Assert
			Assert.IsFalse( unmetCondition );
			Assert.IsTrue( metCondition );
		}

		[TestMethod, TestCategory( "ViewModels.Validation" )]
		public void ValidationMessageIsCorrectlySet()
		{
			// Arrange
			var vm = new TestValidation();

			// Act
			vm.ValidateWrapper( () => vm.Name ).Check( name => false ).Message( "test message" );
			vm.Name = "test";

			// Assert
			Assert.AreEqual( "test message", vm.GetErrors( nameof( vm.Name ) ).Cast<string>().First() );
		}

		[TestMethod, TestCategory( "ViewModels.Validation" )]
		public void ValidationsCanBeRemoved()
		{
			// Arrange
			var vm = new TestValidation();

			vm.ValidateWrapper( () => vm.Name ).Check( name => name != "test" ).Message( "test" );

			// Act
			vm.Name = "test";
			string before = vm.GetErrors( nameof( vm.Name ) ).Cast<string>().First();
			vm.ClearValidationRules();
			vm.Name = "123";
			vm.Name = "test";
			string after = vm.GetErrors( nameof( vm.Name ) ).Cast<string>().FirstOrDefault();

			// Assert
			Assert.AreEqual( "test", before );
			Assert.IsNull( after );
		}

		[TestMethod, TestCategory( "ViewModels.Validation" )]
		public void ValidationWithoutCheckThrowsException()
		{
			// Arrange
			var vm = new TestValidation();

			// Act
			Action action = () => vm.ValidateWrapper( () => vm.Name ).Message( "test" );

			// Assert
			ExceptionAssert.Throws<InvalidOperationException>( action );
		}

		[TestMethod, TestCategory( "ViewModels.Validation" )]
		public void ValidPropertyValueIsValidated()
		{
			// Arrange
			var vm = new TestValidation();

			// Act
			vm.ValidateWrapper( () => vm.Name ).Check( name => true ).Message( "must not be visible" );
			vm.Name = "test";

			// Assert
			Assert.IsFalse( vm.GetErrors( nameof( vm.Name ) ).Cast<string>().Any() );
		}

		private class TestValidation : ValidationViewModel
		{
			public void ValidateAllWrapper()
			{
				ValidateAll();
			}

			public IValidationSetup<TProperty> ValidateWrapper<TProperty>( Expression<Func<TProperty>> propertyName )
			{
				return Validate( propertyName );
			}

			public string Name
			{
				[DebuggerStepThrough] get { return _Name; }
				set
				{
					if( _Name == value )
					{
						return;
					}

					_Name = value;
					RaisePropertyChanged();
				}
			}

			[DebuggerBrowsable( DebuggerBrowsableState.Never )] private string _Name;
		}
	}
}