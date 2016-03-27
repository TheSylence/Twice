using System;
using LinqToTwitter;
using Twice.ViewModels;

namespace Twice.Models.Twitter
{
	internal interface IContextEntry : IDisposable
	{
		string AccountName { get; }
		TwitterContext Twitter { get; }
		ulong UserId { get; }
		Uri ProfileImageUrl { get; }
		INotifier Notifier { get; }
	}
}