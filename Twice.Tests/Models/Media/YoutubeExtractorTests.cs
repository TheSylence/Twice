using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Twice.Models.Media;

namespace Twice.Tests.Models.Media
{
	[TestClass, ExcludeFromCodeCoverage]
	public class YoutubeExtractorTests
	{
		[TestMethod, TestCategory( "Models.Media" )]
		public void SimpleUrlIsMatched()
		{
			// Arrange
			var extractor = new YoutubeExtractor();
			var testCases = new Dictionary<string, bool>
			{
				{"https://www.youtube.com/watch?v=IJhgZBn-LHg", true},
				{"https://www.youtube.com/watch?v=JQVmkDUkZT4", true},
				{"https://youtu.be/JQVmkDUkZT4", true},
				{"https://youtu.be/JQVmkDUkZT4?t=22s", true},
				{"https://www.youtube.com/watch?v=y0opgc1WoS4&list=PLFs4vir_WsTyXrrpFstD64Qj95vpy-yo1&index=3", true},
				{"http://youtube.com/", false},
				{"https://google.com/watch?v=12345", false},
				{"https://youtube.com/watch?v=", false},
				{"http://youtube.com/watch/video/", false},
				{"https://youtube.com/watch?video=12345", false },
				{"https://youtube.com/watch?v=%20&video=123456", false }
			};

			// Act
			var results = testCases.ToDictionary( kvp => kvp.Key, kvp => extractor.CanExtract( kvp.Key ) );

			// Assert
			foreach( var kvp in results )
			{
				Assert.AreEqual( testCases[kvp.Key], kvp.Value, kvp.Key );
			}
		}

		[TestMethod, TestCategory( "Models.Media" )]
		public void CorrectIdIsExtracted()
		{
			// Arrange
			var extractor = new YoutubeExtractor();
			var testCases = new Dictionary<string, string>
			{
				{"https://www.youtube.com/watch?v=IJhgZBn-LHg", "IJhgZBn-LHg"},
				{"https://www.youtube.com/watch?v=JQVmkDUkZT4", "JQVmkDUkZT4"},
				{"https://youtu.be/JQVmkDUkZT4", "JQVmkDUkZT4"},
				{"https://youtu.be/JQVmkDUkZT4?t=22s", "JQVmkDUkZT4"},
				{"https://www.youtube.com/watch?v=y0opgc1WoS4&list=PLFs4vir_WsTyXrrpFstD64Qj95vpy-yo1&index=3", "y0opgc1WoS4"}
			};

			// Act
			var results = testCases.ToDictionary( kvp => kvp.Key, kvp => extractor.GetMediaUrl( kvp.Key ) );

			// Assert
			foreach( var kvp in results )
			{
				var expectedId = testCases[kvp.Key];

				Assert.IsTrue( kvp.Value.AbsoluteUri.Contains( expectedId ), kvp.Key );
			}
		}
	}
}