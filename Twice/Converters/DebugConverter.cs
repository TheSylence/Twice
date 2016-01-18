using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;

namespace Twice.Converters
{
	internal class DebugConverter : IValueConverter
	{
		public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
		{
#if DEBUG
			if( Debugger.IsAttached )
			{
				Debugger.Break();
			}
#endif

			return value;
		}

		public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
		{
#if DEBUG
			if( Debugger.IsAttached )
			{
				Debugger.Break();
			}
#endif

			return value;
		}
	}
}