using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using Twice.ViewModels;

namespace Twice.Converters
{
	internal class IconToVisual : IValueConverter
	{
		public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
		{
			var desc = typeof( Icon ).GetField( value.ToString() ).GetCustomAttribute<DescriptionAttribute>()?.Description;
			Debug.Assert( desc != null, "desc != null" );

			return Application.Current.FindResource( desc );
		}

		public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
		{
			throw new NotSupportedException();
		}
	}
}