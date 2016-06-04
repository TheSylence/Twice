using System;

namespace Twice.ViewModels.Validation
{
	internal class ValidationSetup<TProperty> : IValidationSetup<TProperty>
	{
		public ValidationSetup( IPropertyValidatorContainer propertyValidatorContainer, string propertyName, bool manual )
		{
			PropertyValidatorContainer = propertyValidatorContainer;
			PropertyName = propertyName;
			Condition = () => true;
			IsManual = manual;
		}

		public IValidationSetup<TProperty> Check( Func<TProperty, bool> action )
		{
			CheckFunc = action;
			return this;
		}

		public IValidationSetup<TProperty> If( Func<bool> check )
		{
			Condition = check;
			return this;
		}

		public void Message( string message )
		{
			if( CheckFunc == null )
			{
				throw new InvalidOperationException( "No check constraint set" );
			}

			PropertyValidatorContainer.AddValidator( PropertyName,
				new PropertyValidator<TProperty>( CheckFunc, Condition, message, IsManual ) );
		}

		private readonly bool IsManual;
		private readonly string PropertyName;
		private readonly IPropertyValidatorContainer PropertyValidatorContainer;
		private Func<TProperty, bool> CheckFunc;
		private Func<bool> Condition;
	}
}