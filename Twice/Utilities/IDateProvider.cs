using System;

namespace Twice.Utilities
{
	internal interface IDateProvider
	{
		DateTime Now { get; }
	}
}