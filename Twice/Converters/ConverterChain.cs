using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Twice.Converters
{
	/// <summary>
	/// Convert that is able to "chain" multiple converters together and call them in sequence to use
	/// more than one converter on a value.
	/// </summary>
	internal class ConverterChain : List<IValueConverter>, IValueConverter
	{
		public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
		{
			return this.Aggregate( value, ( current, converter ) => converter.Convert( current, targetType, parameter, culture ) );
		}

		public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
		{
			throw new NotSupportedException();
		}
	}
}