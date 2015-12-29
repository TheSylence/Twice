using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Twice.Converters
{
	internal class InvertBool : IValueConverter
	{
		public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
		{
			return ConvertInternal( value );
		}

		public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
		{
			return ConvertInternal( value );
		}

		private static object ConvertInternal( object value )
		{
			if( !( value is bool ) )
			{
				return DependencyProperty.UnsetValue;
			}

			return !(bool)value;
		}
	}
}