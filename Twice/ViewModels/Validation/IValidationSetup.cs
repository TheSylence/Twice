using System;

namespace Twice.ViewModels.Validation
{
	/// <summary>
	///     Setup object for a validation
	/// </summary>
	public interface IValidationSetup<out TProperty>
	{
		/// <summary>
		///     Sets the action that is executed when validation is done.
		/// </summary>
		/// <param name="action">
		///     The validation action. Return <c> false </c> if validation did not pass.
		/// </param>
		/// <returns> This object. </returns>
		IValidationSetup<TProperty> Check( Func<TProperty, bool> action );

		/// <summary>
		///     Sets a condition that must be met before validation occurs.
		/// </summary>
		/// <param name="check"> The condition that must be met. </param>
		/// <returns> This object. </returns>
		IValidationSetup<TProperty> If( Func<bool> check );

		/// <summary>
		///     Sets the message that should be used when the validation fails.
		/// </summary>
		/// <param name="message"> Message to use. </param>
		void Message( string message );
	}
}