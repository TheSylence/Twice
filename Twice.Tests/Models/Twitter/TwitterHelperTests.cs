using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Twice.Models.Twitter;

namespace Twice.Tests.Models.Twitter
{
	[TestClass, ExcludeFromCodeCoverage]
	public class TwitterHelperTests
	{
		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void MimeTypeForFileIsDetectedCorrectly()
		{
			// Arrange
			var testCases = new Dictionary<string, string>
			{
				{"file.jpg", "image/jpg"},
				{"file.png", "image/png"},
				{"file.bmp", "image/bmp"},
				{"file.gif", "image/gif"},
				{"file.png.gif", "image/gif"},
				{"file", "application/octet-stream"},
				{"name.exe", "application/octet-stream"},
				{"name", "application/octet-stream"}
			};

			// Act
			var results = testCases.ToDictionary( kvp => kvp.Key, kvp => TwitterHelper.GetMimeType( kvp.Key ) );

			// Assert
			foreach( var kvp in results )
			{
				Assert.AreEqual( testCases[kvp.Key], kvp.Value );
			}
		}

		[TestMethod, TestCategory( "Models.Twitter" )]
		public void TweetIdCanBeExtractedFromUrl()
		{
			// Arrange
			var testCases = new Dictionary<string, ulong>
			{
				{"https://twitter.com/TweetAngi/status/733279889596514304", 733279889596514304},
				{"https://twitter.com/dqmhose/status/733279824299630592", 733279824299630592},
				{"https://twitter.com/anne_nymus/status/733274507339763712", 733274507339763712},
				{"https://twitter.com/LaDolceVegas/status/725649743364370433", 725649743364370433},
				{"https://twitter.com/ID_AA_Carmack/status/733275349048381440", 733275349048381440},
				{"https://twitter.com/SPORT1/status/733273819994071040/", 733273819994071040},
				{"https://twitter.com/NetflixDE/status/733279091869286400?variable=true", 733279091869286400},
				{ "https://pbs.twimg.com/media/Ci0gimSWEAASP-L.jpg", 0},
				{"https://twitter.com", 0},
				{"https://twitter.com/twitterapi", 0}
			};

			// Act
			var results = testCases.ToDictionary( kvp => kvp.Key, kvp => TwitterHelper.ExtractTweetId( kvp.Key ) );

			// Assert
			foreach( var kvp in testCases )
			{
				Assert.AreEqual( kvp.Value, results[kvp.Key], kvp.Key );
			}
		}

		[TestMethod, TestCategory( "Models.Twitter" )]
		public void TweetUrlIsCorrectlyRecognized()
		{
			// Arrange
			var testCases = new Dictionary<string, bool>
			{
				{"https://twitter.com/TweetAngi/status/733279889596514304", true},
				{"https://twitter.com/dqmhose/status/733279824299630592", true},
				{"https://twitter.com/anne_nymus/status/733274507339763712", true},
				{"https://twitter.com/LaDolceVegas/status/725649743364370433", true},
				{"https://twitter.com/ID_AA_Carmack/status/733275349048381440", true},
				{"https://twitter.com/SPORT1/status/733273819994071040", true},
				{"https://twitter.com/NetflixDE/status/733279091869286400", true},
				{"https://pbs.twimg.com/media/Ci0gimSWEAASP-L.jpg", false},
				{"https://twitter.com", false},
				{"https://twitter.com/twitterapi", false}
			};

			// Act
			var results = testCases.ToDictionary( kvp => kvp.Key, kvp => TwitterHelper.IsTweetUrl( new System.Uri( kvp.Key ) ) );

			// Assert
			foreach( var kvp in testCases )
			{
				Assert.AreEqual( kvp.Value, results[kvp.Key], kvp.Key );
			}
		}
	}
}