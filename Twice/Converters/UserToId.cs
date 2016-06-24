using LinqToTwitter;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Twice.Models.Twitter;
using Twice.ViewModels.Twitter;

namespace Twice.Converters
{
	internal class UserToId : IValueConverter
	{
		public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
		{
			User user = value as User;
			if( user == null )
			{
				var vm = value as UserViewModel;
				if( vm != null )
				{
					user = vm.Model;
				}
			}

			return user?.GetUserId() ?? DependencyProperty.UnsetValue;
		}

		public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
		{
			throw new NotSupportedException();
		}
	}
}