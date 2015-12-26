using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Twice.Models.Twitter;

namespace Twice.Converters
{
	internal class SourceName : IValueConverter
	{
		public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
		{
			string str = value as string;

			if( str == null )
			{
				return DependencyProperty.UnsetValue;
			}

			return new TweetSource( str ).Name;
		}

		public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
		{
			throw new NotSupportedException();
		}
	}

	internal class SourceUrl : IValueConverter
	{
		public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
		{
			string str = value as string;

			if( str == null )
			{
				return DependencyProperty.UnsetValue;
			}

			return new TweetSource( str ).Url;
		}

		public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
		{
			throw new NotSupportedException();
		}
	}
}
