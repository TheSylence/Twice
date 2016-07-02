using System;
using System.Globalization;
using System.Windows.Data;

namespace Twice.Converters
{
	internal class MnemonicsRemover : IValueConverter
	{
		public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
		{
			string strValue = value as string;
			return strValue?.Replace( "_", "__" ) ?? value;
		}

		public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
		{
			throw new NotSupportedException();
		}
	}
}