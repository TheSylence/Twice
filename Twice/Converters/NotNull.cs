using System;
using System.Globalization;
using System.Windows.Data;

namespace Twice.Converters
{
	internal class NotNull : IValueConverter
	{
		public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
		{
			return value != null;
		}

		public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
		{
			throw new NotSupportedException();
		}
	}
}