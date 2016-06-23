using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twice.Models.Twitter;
using Twice.ViewModels;

namespace Twice.Tests.Models.Twitter
{
	[TestClass, ExcludeFromCodeCoverage]
	public class ContextEntryTests
	{
		[TestMethod, TestCategory( "Models.Twitter" )]
		public void DisposingEntryDisposesContext()
		{
			// Arrange
			var context = new Mock<ITwitterContext>();
			context.Setup( c => c.Dispose() ).Verifiable();

			var entry = new ContextEntry( null, null, null, context.Object );

			// Act
			entry.Dispose();

			// Assert
			context.Verify( c => c.Dispose(), Times.Once() );
		}

		[TestMethod, TestCategory( "Models.Twitter" )]
		public void EqualsCheckForUserId()
		{
			// Arrange
			var notifier = new Mock<INotifier>();
			var data = new TwitterAccountData
			{
				UserId = 123,
				ImageUrl = "http://example.com/image.png"
			};
			var a = new ContextEntry( notifier.Object, data, null );
			var b = new ContextEntry( notifier.Object, data, null );
			var c = new ContextEntry( notifier.Object, new TwitterAccountData
			{
				UserId = 111,
				ImageUrl = "http://example.com/image.png"
			}, null );

			// Act
			var ab = a.Equals( b );
			var ba = b.Equals( a );
			var ac = a.Equals( c );

			// ReSharper disable once SuspiciousTypeConversion.Global
			var type = a.Equals( string.Empty );

			// Assert
			Assert.IsTrue( ab );
			Assert.IsTrue( ba );
			Assert.IsFalse( ac );
			Assert.IsFalse( type );
		}

		[TestMethod, TestCategory( "Models.Twitter" )]
		public void HashCodeIsBaedOnUserId()
		{
			// Arrange
			var notifier = new Mock<INotifier>();
			var data = new TwitterAccountData
			{
				UserId = 123,
				ImageUrl = "http://example.com/image.png"
			};
			var entry = new ContextEntry( notifier.Object, data, null );

			// Act
			var hash = entry.GetHashCode();

			// Assert
			Assert.AreEqual( entry.UserId.GetHashCode(), hash );
		}
	}
}