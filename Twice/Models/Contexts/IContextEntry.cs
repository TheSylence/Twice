using System;
using LinqToTwitter;

namespace Twice.Models.Contexts
{
	internal interface IContextEntry : IDisposable
	{
		string AccountName { get; }
		TwitterContext Twitter { get; }
		ulong UserId { get; }
	}
}