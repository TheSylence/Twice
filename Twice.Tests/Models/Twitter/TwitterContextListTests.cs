using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Twice.Models.Twitter;
using Twice.Utilities;
using Twice.ViewModels;

namespace Twice.Tests.Models.Twitter
{
	[TestClass, ExcludeFromCodeCoverage]
	public class TwitterContextListTests
	{
		[TestMethod, TestCategory( "Models.Twitter" )]
		public void DisposingContextListDisposesContexts()
		{
			// Arrange
			string fileName = Path.GetTempFileName();
			var notifier = new Mock<INotifier>();
			var serializer = new Mock<ISerializer>();
			var list = new TwitterContextList( notifier.Object, fileName, serializer.Object, null );

			var context = new Mock<IContextEntry>();
			context.Setup( c => c.Dispose() ).Verifiable();
			list.Contexts.Add( context.Object );

			// Act
			list.Dispose();

			// Assert
			context.Verify( c => c.Dispose(), Times.Once() );
		}

		[TestMethod, TestCategory( "Models.Twitter" )]
		public void LoadingFromNonExistingFileDoesNothing()
		{
			// Arrange
			string fileName = Path.GetTempFileName() + "a";
			var notifier = new Mock<INotifier>();
			var serializer = new Mock<ISerializer>( MockBehavior.Strict );

			// Act ReSharper disable once ObjectCreationAsStatement
			var ex = ExceptionAssert.Catch<Exception>( () => new TwitterContextList( notifier.Object, fileName, serializer.Object, null ) );

			// Assert
			Assert.IsNull( ex );
		}

		[TestMethod, TestCategory( "Models.Twitter" )]
		public void NewAccountCanBeAdded()
		{
			// Arrange
			string fileName = Path.GetTempFileName();
			var notifier = new Mock<INotifier>();
			var list = new TwitterContextList( notifier.Object, fileName, new Serializer(), null );

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