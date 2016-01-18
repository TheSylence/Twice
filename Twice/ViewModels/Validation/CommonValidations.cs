using System.Collections.Generic;
using System.Linq;
using Twice.Resources;

namespace Twice.ViewModels.Validation
{
	internal static class CommonValidations
	{
		public static void NotEmpty( this IValidationSetup<string> setup, bool allowWhitespaces = false )
		{
			setup.Check( v =>
			{
				if( allowWhitespaces )
				{
					return !string.IsNullOrEmpty( v );
				}

				return !string.IsNullOrWhiteSpace( v );
			} ).Message( Strings.ValueMustNotBeEmpty );
		}

		public static void NotEmpty<T>( this IValidationSetup<T?> setup ) where T : struct
		{
			setup.Check( v => v.HasValue ).Message( Strings.ValueMustNotBeEmpty );
		}

		public static void SelectedNotNull<TProperty>( this IValidationSetup<TProperty> setup ) where TProperty : class
		{
			setup.Check( v => v != null ).Message( Strings.SelectAValue );
		}

		public static void Unique( this IValidationSetup<string> setup, IEnumerable<string> existingValues )
		{
			setup.Check( v => !existingValues.Contains( v ) )
				.Message( Strings.NameAlreadyExists );
		}
	}
}