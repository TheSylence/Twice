using System;
using LinqToTwitter;

namespace Twice.Models.Contexts
{
	internal interface IContextEntry : IDisposable
	{
		string AccountName { get; }
		TwitterContext Context { get; }
		ulong UserId { get; }
	}
}