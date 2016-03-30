using LinqToTwitter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twice.Models.Configuration;
using Twice.Models.Twitter;

namespace Twice.Tests.Models.Twitter
{
	[TestClass]
	public class StatusMuterTests
	{
		[TestMethod, TestCategory( "Models.Twitter" )]
		public void EmptyMutingsWillNotMatchAnything()
		{
			// Arrange
			var muteConfig = new MuteConfig();
			var config = new Mock<IConfig>();
			config.SetupGet( c => c.Mute ).Returns( muteConfig );

			var muter = new StatusMuter( config.Object );
			var status = new Status
			{
				Text = "Hello World"
			};

			// Act
			bool match = muter.IsMuted( status );

			// Assert
			Assert.IsFalse( match );
		}

		[TestMethod, TestCategory( "Models.Twitter" )]
		public void NullStatusIsRejected()
		{
			// Arrange
			var muteConfig = new MuteConfig();
			var config = new Mock<IConfig>();
			config.SetupGet( c => c.Mute ).Returns( muteConfig );

			var muter = new StatusMuter( config.Object );

			// Act
			bool match = muter.IsMuted( null );

			// Assert
			Assert.IsFalse( match );
		}

		[TestMethod, TestCategory( "Models.Twitter" )]
		public void TextMuteIsCorrectlyMatched()
		{
			// Arrange
			var muteConfig = new MuteConfig();
			muteConfig.Entries.Add( new MuteEntry {Filter = "test"} );

			var config = new Mock<IConfig>();
			config.SetupGet( c => c.Mute ).Returns( muteConfig );

			var muter = new StatusMuter( config.Object );

			var status = new Status
			{
				Text = "Will not match"
			};

			// Act
			var noMatch = muter.IsMuted( status );

			status.Text = "Will match since test is contained";
			var match = muter.IsMuted( status );

			// Assert
			Assert.IsFalse( noMatch );
			Assert.IsTrue( match );
		}
	}
}