using System;

namespace Twice.Utilities
{
	internal class DateProvider : IDateProvider
	{
		internal static readonly IDateProvider Default = new DateProvider();

		public DateTime Now => DateTime.Now;
	}
}