using LinqToTwitter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Twice.Models.Configuration;
using Twice.Models.Twitter;

namespace Twice.Tests.Models.Twitter
{
	[TestClass, ExcludeFromCodeCoverage]
	public class StatusMuterTests
	{
		[TestMethod, TestCategory( "Models.Twitter" )]
		public void CasingIsAppliedDuringTextMuting()
		{
			// Arrange
			var muteConfig = new MuteConfig();
			muteConfig.Entries.Add( new MuteEntry { Filter = "Text", CaseSensitive = true } );

			var config = new Mock<IConfig>();
			config.SetupGet( c => c.Mute ).Returns( muteConfig );

			var muter = new StatusMuter( config.Object );

			// Act
			bool matchedCaseWhenSensitive = muter.IsMuted( new Status { Text = "This is a Text" } );
			bool unmatchedCaseWhenSensitive = muter.IsMuted( new Status { Text = "This is a text" } );

			muteConfig.Entries[0].CaseSensitive = false;
			bool matchedCaseWhenInsensitive = muter.IsMuted( new Status { Text = "This is a Text" } );
			bool unmatchedCaseWhenInsensitive = muter.IsMuted( new Status { Text = "This is a text" } );

			// Assert
			Assert.IsTrue( matchedCaseWhenInsensitive );
			Assert.IsTrue( matchedCaseWhenSensitive );
			Assert.IsTrue( unmatchedCaseWhenInsensitive );
			Assert.IsFalse( unmatchedCaseWhenSensitive );
		}

		[TestMethod, TestCategory( "Models.Twitter" )]
		public void CasingIsAppliedForHashtags()
		{
			// Arrange
			var muteConfig = new MuteConfig();
			muteConfig.Entries.Add( new MuteEntry { Filter = "#Text", CaseSensitive = true } );

			var config = new Mock<IConfig>();
			config.SetupGet( c => c.Mute ).Returns( muteConfig );

			var muter = new StatusMuter( config.Object );

			var upperStatus = new Status
			{
				Entities = new Entities
				{
					HashTagEntities = new List<HashTagEntity>
					{
						new HashTagEntity
						{
							Tag = "Text"
						}
					}
				}
			};

			var lowerStatus = new Status
			{
				Entities = new Entities
				{
					HashTagEntities = new List<HashTagEntity>
					{
						new HashTagEntity
						{
							Tag = "text"
						}
					}
				}
			};

			// Act
			bool matchedCaseWhenSensitive = muter.IsMuted( upperStatus );
			bool unmatchedCaseWhenSensitive = muter.IsMuted( lowerStatus );

			muteConfig.Entries[0].CaseSensitive = false;
			bool matchedCaseWhenInsensitive = muter.IsMuted( upperStatus );
			bool unmatchedCaseWhenInsensitive = muter.IsMuted( lowerStatus );

			// Assert
			Assert.IsTrue( matchedCaseWhenInsensitive );
			Assert.IsTrue( matchedCaseWhenSensitive );
			Assert.IsTrue( unmatchedCaseWhenInsensitive );
			Assert.IsFalse( unmatchedCaseWhenSensitive );
		}

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
		public void HashtagMutesAreCorrectlyMatched()
		{
			// Arrange
			var muteConfig = new MuteConfig();
			muteConfig.Entries.Add( new MuteEntry { Filter = "#sameTag" } );
			muteConfig.Entries.Add( new MuteEntry { Filter = "#differentTag" } );

			var config = new Mock<IConfig>();
			config.SetupGet( c => c.Mute ).Returns( muteConfig );

			var muter = new StatusMuter( config.Object );

			// Act
			bool noTag = muter.IsMuted( new Status
			{
				Entities = new Entities
				{
					HashTagEntities = new List<HashTagEntity>()
				}
			} );

			bool otherTag = muter.IsMuted( new Status
			{
				Entities = new Entities
				{
					HashTagEntities = new List<HashTagEntity>
					{
						new HashTagEntity {Tag = "otherTag"}
					}
				}
			} );

			bool sameTag = muter.IsMuted( new Status
			{
				Entities = new Entities
				{
					HashTagEntities = new List<HashTagEntity>
					{
						new HashTagEntity {Tag = "sameTag"}
					}
				}
			} );

			// Assert
			Assert.IsFalse( noTag );
			Assert.IsFalse( otherTag );
			Assert.IsTrue( sameTag );
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
			Assert.IsTrue( match );
		}

		[TestMethod, TestCategory( "Models.Twitter" )]
		public void TextMuteIsCorrectlyMatched()
		{
			// Arrange
			var muteConfig = new MuteConfig();
			muteConfig.Entries.Add( new MuteEntry { Filter = "test" } );

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

		[TestMethod, TestCategory( "Models.Twitter" )]
		public void UserMutesAreCorrectlyMatched()
		{
			// Arrange
			var muteConfig = new MuteConfig();
			muteConfig.Entries.Add( new MuteEntry { Filter = "@sameUser" } );
			muteConfig.Entries.Add( new MuteEntry { Filter = "@differentUser" } );

			var config = new Mock<IConfig>();
			config.SetupGet( c => c.Mute ).Returns( muteConfig );

			var muter = new StatusMuter( config.Object );

			// Act
			bool noUser = muter.IsMuted( new Status
			{
				Entities = new Entities
				{
					UserMentionEntities = new List<UserMentionEntity>()
				}
			} );

			bool otherUser = muter.IsMuted( new Status
			{
				Entities = new Entities
				{
					UserMentionEntities = new List<UserMentionEntity>
					{
						new UserMentionEntity {ScreenName = "otherUser"}
					}
				}
			} );

			bool sameUser = muter.IsMuted( new Status
			{
				Entities = new Entities
				{
					UserMentionEntities = new List<UserMentionEntity>
					{
						new UserMentionEntity {ScreenName = "sameUser"}
					}
				}
			} );

			// Assert
			Assert.IsFalse( noUser );
			Assert.IsFalse( otherUser );
			Assert.IsTrue( sameUser );
		}
	}
}