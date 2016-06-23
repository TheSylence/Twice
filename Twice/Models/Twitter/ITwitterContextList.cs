using System;
using System.Collections.Generic;

namespace Twice.Models.Twitter
{
	internal interface ITwitterContextList : IDisposable
	{
		event EventHandler ContextsChanged;

		/// <summary>
		///     Add a new account to the list. Only pass decrypted data to this. They will be encrypted
		///     when the methd returns.
		/// </summary>
		/// <param name="data"></param>
		void AddContext( TwitterAccountData data );

		/// <summary>
		///     Only pass decrypted data to this method.
		/// </summary>
		/// <param name="data"></param>
		void UpdateAccount( TwitterAccountData data );

		/// <summary>
		/// Removes an account.
		/// </summary>
		/// <param name="userId"></param>
		void RemoveAccount( ulong userId );

		void UpdateAllAccounts();

		ICollection<IContextEntry> Contexts { get; }
	}
}