using System;
using System.Collections.Generic;

namespace Twice.Models.Contexts
{
	internal interface ITwitterContextList : IDisposable
	{
		ICollection<IContextEntry> Contexts { get; }
	}
}