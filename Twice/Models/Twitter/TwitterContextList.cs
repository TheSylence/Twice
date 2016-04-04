using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Twice.ViewModels;

namespace Twice.Models.Twitter
{
	internal class TwitterContextList : ITwitterContextList
	{
		public TwitterContextList( INotifier notifier, string fileName )
		{
			FileName = fileName;
			Notifier = notifier;
			Contexts = new List<IContextEntry>();

			if( File.Exists( FileName ) )
			{
				var json = File.ReadAllText( FileName );
				List<TwitterAccountData> accountData;

				try
				{
					accountData = JsonConvert.DeserializeObject<List<TwitterAccountData>>( json );
				}
				catch( JsonReaderException )
				{
					accountData = new List<TwitterAccountData>();
				}

				Contexts = accountData.Select( acc =>
				{
					acc.Decrypt();
					try
					{
						var ctx = new ContextEntry( Notifier, acc );

						return ctx;
					}
					finally
					{
						acc.Encrypt();
					}
				} ).Cast<IContextEntry>().ToList();
			}
		}

		public event EventHandler ContextsChanged;

		public void AddContext( TwitterAccountData data )
		{
			Contexts.Add( new ContextEntry( Notifier, data ) );

			SaveToFile();

			ContextsChanged?.Invoke( this, EventArgs.Empty );
		}

		public void Dispose()
		{
			Dispose( true );
			GC.SuppressFinalize( this );
		}

		/// <summary>
		///     Only pass encrypted data to this method.
		/// </summary>
		/// <param name="data"></param>
		public void UpdateAccount( TwitterAccountData data )
		{
			var context = Contexts.FirstOrDefault( c => c.UserId == data.UserId );
			Contexts.Remove( context );

			Contexts.Add( new ContextEntry( Notifier, data ) );
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
			var json = JsonConvert.SerializeObject( Contexts.Select( ctx =>
			{
				var result = new TwitterAccountData
				{
					AccountName = ctx.AccountName,
					ImageUrl = ctx.ProfileImageUrl.AbsoluteUri,
					OAuthToken = ctx.Twitter.Authorizer.CredentialStore.OAuthToken,
					OAuthTokenSecret = ctx.Twitter.Authorizer.CredentialStore.OAuthTokenSecret,
					UserId = ctx.UserId
				};

				result.Encrypt();
				return result;
			} ).ToList(), Formatting.Indented );

			File.WriteAllText( FileName, json );
		}

		public ICollection<IContextEntry> Contexts { get; }
		private readonly string FileName;
		private readonly INotifier Notifier;
	}
}