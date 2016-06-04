using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;
using Twice.Attributes;
using Twice.ViewModels;

namespace Twice.Converters
{
	internal class IconToVisual : IValueConverter
	{
		public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
		{
			var kind = typeof(Icon).GetField( value.ToString() ).GetCustomAttribute<IconAttribute>()?.Kind;
			Debug.Assert( kind != null, "kind != null" );

			return kind;
		}

		public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
		{
			throw new NotSupportedException();
		}
	}
}