using System;
using System.Diagnostics.CodeAnalysis;

namespace Twice.Utilities
{
	[ExcludeFromCodeCoverage]
	internal class DateProvider : IDateProvider
	{
		internal static readonly IDateProvider Default = new DateProvider();

		public DateTime Now => DateTime.Now;
	}
}