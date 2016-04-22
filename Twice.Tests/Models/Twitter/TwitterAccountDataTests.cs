using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Twice.Models.Twitter;
using Twice.Utilities;

namespace Twice.Tests.Models.Twitter
{
	[TestClass]
	public class TwitterAccountDataTests
	{
		[TestMethod, TestCategory( "Models.Twitter" )]
		public void DecryptedActionEncryptsAfterExecution()
		{
			// Arrange
			var data = new TwitterAccountData
			{
				OAuthToken = DpApi.Encrypt( "token" ),
				OAuthTokenSecret = DpApi.Encrypt( "secret" )
			};

			// Act
			data.ExecuteDecryptedAction( d => { } );

			// Assert
			Assert.AreNotEqual( "token", data.OAuthToken );
			Assert.AreNotEqual( "secret", data.OAuthTokenSecret );
		}

		[TestMethod, TestCategory( "Models.Twitter" )]
		public void DecryptionOnlyDecryptsTokens()
		{
			// Arrange
			string name = "name";
			string url = "url";
			string token = DpApi.Encrypt( "the oauth token" );
			string secret = DpApi.Encrypt( "the token secret" );
			ulong userId = 123456;

			var data = new TwitterAccountData
			{
				AccountName = name,
				ImageUrl = url,
				IsDefault = true,
				OAuthToken = token,
				OAuthTokenSecret = secret,
				RequiresConfirm = true,
				UserId = userId
			};

			// Act
			data.Decrypt();

			// Assert
			Assert.AreEqual( name, data.AccountName );
			Assert.AreEqual( url, data.ImageUrl );
			Assert.AreEqual( "the oauth token", data.OAuthToken );
			Assert.AreEqual( "the token secret", data.OAuthTokenSecret );
			Assert.AreEqual( userId, data.UserId );
			Assert.IsTrue( data.IsDefault );
			Assert.IsTrue( data.RequiresConfirm );
		}

		[TestMethod, TestCategory( "Models.Twitter" )]
		public void EncryptedActionDecryptsBeforeExecution()
		{
			// Arrange
			var data = new TwitterAccountData
			{
				OAuthToken = DpApi.Encrypt( "token" ),
				OAuthTokenSecret = DpApi.Encrypt( "secret" )
			};

			string decryptedToken = "";
			string decryptedSecret = "";
			Action<TwitterAccountData> action = d =>
			{
				decryptedToken = d.OAuthToken;
				decryptedSecret = d.OAuthTokenSecret;
			};

			// Act
			data.ExecuteDecryptedAction( action );

			// Assert
			Assert.AreEqual( "token", decryptedToken );
			Assert.AreEqual( "secret", decryptedSecret );
		}

		[TestMethod, TestCategory( "Models.Twitter" )]
		public void EncryptedFuncPassesThroughReturnValue()
		{
			// Arrange

			var data = new TwitterAccountData
			{
				OAuthToken = DpApi.Encrypt( "token" ),
				OAuthTokenSecret = DpApi.Encrypt( "secret" )
			};

			Func<TwitterAccountData, int> action = d => 42;

			// Act
			var value = data.ExecuteDecryptedAction( action );

			// Assert
			Assert.AreEqual( 42, value );
		}

		[TestMethod, TestCategory( "Models.Twitter" )]
		public void EncryptionOnlyEncryptsTokens()
		{
			// Arrange
			string name = "name";
			string url = "url";
			string token = "the oauth token";
			string secret = "the token secret";
			ulong userId = 123456;

			var data = new TwitterAccountData
			{
				AccountName = name,
				ImageUrl = url,
				IsDefault = true,
				OAuthToken = token,
				OAuthTokenSecret = secret,
				RequiresConfirm = true,
				UserId = userId
			};

			// Act
			data.Encrypt();

			// Assert
			Assert.AreEqual( name, data.AccountName );
			Assert.AreEqual( url, data.ImageUrl );
			Assert.AreNotEqual( token, data.OAuthToken );
			Assert.AreNotEqual( secret, data.OAuthTokenSecret );
			Assert.AreEqual( userId, data.UserId );
			Assert.IsTrue( data.IsDefault );
			Assert.IsTrue( data.RequiresConfirm );
		}
	}
}