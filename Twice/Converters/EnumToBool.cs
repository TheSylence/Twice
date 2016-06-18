using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Twice.Converters
{
	internal class EnumToBool : IValueConverter
	{
		public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
		{
			string parameterString = parameter as string;
			if( parameterString == null )
			{
				return DependencyProperty.UnsetValue;
			}

			try
			{
				if( Enum.IsDefined( value.GetType(), value ) == false )
				{
					return DependencyProperty.UnsetValue;
				}
			}
			catch( ArgumentException )
			{
				return DependencyProperty.UnsetValue;
			}

			object parameterValue = Enum.Parse( value.GetType(), parameterString );

			return parameterValue.Equals( value );
		}

		public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
		{
			string parameterString = parameter as string;
			return parameterString == null
				? DependencyProperty.UnsetValue
				: Enum.Parse( targetType, parameterString );
		}
	}
}