using System;
using LinqToTwitter;
using Twice.ViewModels;

namespace Twice.Models.Twitter
{
	internal interface IContextEntry : IDisposable
	{
		string AccountName { get; }
		TwitterAccountData Data { get; }
		bool IsDefault { get; }
		INotifier Notifier { get; }
		Uri ProfileImageUrl { get; }
		bool RequiresConfirmation { get; }
		TwitterContext Twitter { get; }
		ulong UserId { get; }
	}
}