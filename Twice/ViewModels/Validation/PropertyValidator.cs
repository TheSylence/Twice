using System;

namespace Twice.ViewModels.Validation
{
	internal class PropertyValidator<TProperty> : PropertyValidatorBase
	{
		public PropertyValidator( Func<TProperty, bool> check, Func<bool> condition, string erorrMessage, bool isManual = false )
			: base( isManual )
		{
			Check = check;
			ErrorMessage = erorrMessage;
			Condition = condition;
		}

		public override void Update( object value )
		{
			UpdateInternal( (TProperty)value );
		}

		private void UpdateInternal( TProperty value )
		{
			Error = null;
			HasError = false;
			if( !Condition() )
			{
				return;
			}

			try
			{
				if( !Check( value ) )
				{
					Error = ErrorMessage;
					HasError = true;
				}
			}
			catch( Exception ex )
			{
				HasError = true;
				Error = ex.Message;
			}
		}

		private readonly Func<TProperty, bool> Check;
		private readonly Func<bool> Condition;
		private readonly string ErrorMessage;
	}
}