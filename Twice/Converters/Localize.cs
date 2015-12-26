using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Twice.Converters
{
	internal class Localize : IValueConverter
	{
		public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
		{
			string format = parameter as string;

			if( value == null || format == null )
			{
				return DependencyProperty.UnsetValue;
			}

			return string.Format( culture, format, value );
		}

		public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
		{
			throw new NotSupportedException();
		}
	}
}