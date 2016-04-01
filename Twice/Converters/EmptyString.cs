﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace Twice.Converters
{
	internal class EmptyString : IValueConverter
	{
		public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
		{
			return string.IsNullOrEmpty( value as string );
		}

		public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
		{
			throw new NotSupportedException();
		}
	}
}