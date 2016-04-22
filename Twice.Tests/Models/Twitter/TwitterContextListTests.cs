using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.IO;
using Twice.Models.Twitter;
using Twice.ViewModels;

namespace Twice.Tests.Models.Twitter
{
	[TestClass]
	public class TwitterContextListTests
	{
		[TestMethod, TestCategory( "Models.Twitter" )]
		public void NewAccountCanBeAdded()
		{
			// Arrange
			string fileName = Path.GetTempFileName();
			var notifier = new Mock<INotifier>();
			var list = new TwitterContextList( notifier.Object, fileName );

			var toAdd = new TwitterAccountData
			{
				AccountName = "TheName",
				ImageUrl = "http://example.com/image.url",
				IsDefault = true,
				OAuthToken = "TheAuthToken",
				OAuthTokenSecret = "TheAuthSecret",
				RequiresConfirm = true,
				UserId = 12345
			};

			// Act
			list.AddContext( toAdd );

			// Assert
			var fileContent = File.ReadAllText( fileName );

			Assert.IsTrue( fileContent.Contains( toAdd.AccountName ) );
			Assert.IsTrue( fileContent.Contains( toAdd.ImageUrl ) );
			Assert.IsFalse( fileContent.Contains( toAdd.OAuthToken ) );
			Assert.IsFalse( fileContent.Contains( toAdd.OAuthTokenSecret ) );
			Assert.IsTrue( fileContent.Contains( toAdd.UserId.ToString() ) );
		}
	}
}