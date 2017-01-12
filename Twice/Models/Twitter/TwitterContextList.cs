using Anotar.NLog;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Twice.Models.Cache;
using Twice.Utilities;
using Twice.ViewModels;

namespace Twice.Models.Twitter
{
	internal class TwitterContextList : ITwitterContextList
	{
		public TwitterContextList( INotifier notifier, string fileName, ISerializer serializer, ICache cache )
		{
			Cache = cache;
			Serializer = serializer;
			FileName = fileName;
			Notifier = notifier;
			Contexts = new List<IContextEntry>();

			if( !File.Exists( FileName ) )
			{
				return;
			}

			var json = File.ReadAllText( FileName );
			List<TwitterAccountData> accountData;

			try
			{
				accountData = Serializer.Deserialize<List<TwitterAccountData>>( json ) ??
							new List<TwitterAccountData>();
			}
			catch( JsonReaderException )
			{
				accountData = new List<TwitterAccountData>();
			}

			Contexts = accountData.Select( acc =>
			{
				return acc.ExecuteDecryptedAction<IContextEntry>( accDecrypted =>
				{
					var ctx = new ContextEntry( Notifier, accDecrypted, cache );
					LogTo.Info( $"Loaded context for {accDecrypted.AccountName} ({accDecrypted.UserId})" );

					return ctx;
				} );
			} ).ToList();
		}

		public event EventHandler ContextsChanged;

		public void AddContext( TwitterAccountData data )
		{
			LogTo.Info( $"Adding account data for {data.AccountName} ({data.UserId})" );
			Contexts.Add( new ContextEntry( Notifier, data, Cache ) );

			SaveToFile();

			ContextsChanged?.Invoke( this, EventArgs.Empty );
		}

		public void Dispose()
		{
			Dispose( true );
			GC.SuppressFinalize( this );
		}

		public void RemoveAccount( ulong userId )
		{
			LogTo.Info( $"Removing account {userId}" );
			var toRemove = Contexts.FirstOrDefault( c => c.UserId == userId );
			if( toRemove == null )
			{
				LogTo.Warn( "Account wasn't found" );
				return;
			}

			Contexts.Remove( toRemove );
			SaveToFile();
		}

		/// <summary>
		///  Only pass decrypted data to this method. 
		/// </summary>
		/// <param name="data"></param>
		public void UpdateAccount( TwitterAccountData data )
		{
			LogTo.Info( $"Updating account data for {data.AccountName} ({data.UserId})" );
			var context = Contexts.FirstOrDefault( c => c.UserId == data.UserId );
			Contexts.Remove( context );

			Contexts.Add( new ContextEntry( Notifier, data, Cache ) );
			SaveToFile();
		}

		public void UpdateAllAccounts()
		{
			SaveToFile();
		}

		private void Dispose( bool disposing )
		{
			if( disposing )
			{
				foreach( var context in Contexts )
				{
					context.Dispose();
				}
			}
		}

		private void SaveToFile()
		{
			var json = Serializer.Serialize( Contexts.Select( ctx =>
			{
				var result = new TwitterAccountData
				{
					AccountName = ctx.AccountName,
					ImageUrl = ctx.ProfileImageUrl.AbsoluteUri,
					OAuthToken = ctx.Twitter.Authorizer.CredentialStore.OAuthToken,
					OAuthTokenSecret = ctx.Twitter.Authorizer.CredentialStore.OAuthTokenSecret,
					UserId = ctx.UserId,
					IsDefault = ctx.IsDefault,
					RequiresConfirm = ctx.RequiresConfirmation
				};

				result.Encrypt();
				return result;
			} ).ToList() );

			File.WriteAllText( FileName, json );
		}

		public ICollection<IContextEntry> Contexts { get; }
		private readonly ICache Cache;
		private readonly string FileName;
		private readonly INotifier Notifier;
		private readonly ISerializer Serializer;
	}
}