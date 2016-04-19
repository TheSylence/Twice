using System;
using System.Diagnostics.CodeAnalysis;

namespace Twice.Utilities
{
	[ExcludeFromCodeCoverage]
	internal class DateProvider : IDateProvider
	{
		public DateTime Now => DateTime.Now;
		internal static readonly IDateProvider Default = new DateProvider();
	}
}