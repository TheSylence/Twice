using System;
using Twice.ViewModels;

namespace Twice.Models.Twitter
{
	internal interface IContextEntry : IDisposable
	{
		string AccountName { get; }
		TwitterAccountData Data { get; }
		bool IsDefault { get; set; }
		INotifier Notifier { get; }
		Uri ProfileImageUrl { get; }
		bool RequiresConfirmation { get; }
		ITwitterContext Twitter { get; }
		ulong UserId { get; }
	}
}