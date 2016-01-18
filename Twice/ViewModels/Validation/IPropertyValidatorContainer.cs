namespace Twice.ViewModels.Validation
{
	internal interface IPropertyValidatorContainer
	{
		void AddValidator<TProperty>( string propertyName, PropertyValidator<TProperty> validator );
	}
}