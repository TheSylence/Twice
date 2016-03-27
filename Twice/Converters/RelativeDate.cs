using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Twice.Resources;

namespace Twice.Converters
{
	internal class RelativeDate : IValueConverter
	{
		public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
		{
			if( !( value is DateTime ) )
			{
				return DependencyProperty.UnsetValue;
			}

			DateTime date = (DateTime)value;
			date = date.ToLocalTime();

			TimeSpan maxRelative = TimeSpan.MaxValue;

			TimeSpan diff = DateTime.Now - date;

			if( diff > maxRelative )
			{
				return date.ToString( culture );
			}

			int amount = 0;
			string suffix = string.Empty;

			// days
			if( (int)diff.TotalDays >= 1 )
			{
				suffix = Strings.DaysShort;
				amount = (int)diff.TotalDays;
			}
			// hours
			else if( (int)diff.TotalHours >= 1 )
			{
				suffix = Strings.HoursShort;
				amount = (int)diff.TotalHours;
			}
			// minutes
			else if( (int)diff.TotalMinutes >= 1 )
			{
				suffix = Strings.MinutesShort;
				amount = (int)diff.TotalMinutes;
			}
			else
			{
				return Strings.Now;
			}

			//return string.Format( Strings.Ago, string.Format( culture, "{0}{1}", amount, suffix ) );
			return string.Format( culture, "{0}{1}", amount, suffix );
		}

		public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
		{
			throw new NotSupportedException();
		}
	}
}