using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Twice.Converters
{
	internal class UtcToLocal : IValueConverter
	{
		public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
		{
			if( !( value is DateTime ) )
			{
				return DependencyProperty.UnsetValue;
			}

			return new DateTime( ( (DateTime)value ).Ticks, DateTimeKind.Utc ).ToLocalTime();
		}

		public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
		{
			if( !( value is DateTime ) )
			{
				return DependencyProperty.UnsetValue;
			}

			return new DateTime( ( (DateTime)value ).Ticks, DateTimeKind.Local ).ToUniversalTime();
		}
	}
}