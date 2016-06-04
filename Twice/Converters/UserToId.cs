using LinqToTwitter;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Twice.Models.Twitter;

namespace Twice.Converters
{
	internal class UserToId : IValueConverter
	{
		public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
		{
			User user = value as User;
			if( user == null )
			{
				return DependencyProperty.UnsetValue;
			}

			return user.GetUserId();
		}

		public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
		{
			throw new NotSupportedException();
		}
	}
}