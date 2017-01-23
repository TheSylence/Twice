using LinqToTwitter;
using System;
using System.Globalization;
using System.Windows.Data;
using Twice.Models.Twitter;

namespace Twice.Converters
{
	/// <summary>
	///  Converter that converts a tweet to an URL that points to the tweet. 
	/// </summary>
	internal class StatusLink : IValueConverter
	{
		public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
		{
			Status status = value as Status;
			return status == null
				? value
				: status.GetUrl();
		}

		public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
		{
			throw new NotSupportedException();
		}
	}
}