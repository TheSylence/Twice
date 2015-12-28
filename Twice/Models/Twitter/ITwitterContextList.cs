using System;
using System.Collections.Generic;

namespace Twice.Models.Twitter
{
	internal interface ITwitterContextList : IDisposable
	{
		ICollection<IContextEntry> Contexts { get; }
	}
}