using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Twice.Models.Media;

namespace Twice.Tests.Models.Media
{
	[TestClass, ExcludeFromCodeCoverage]
	public class InstragramExtractorTests
	{
		[TestMethod, TestCategory( "Models.Media" )]
		public void InstagramUrlsAreIdentifiedCorrectly()
		{
			// Arrange
			var extractor = new InstragramExtractor();

			var testCases = new Dictionary<string, bool>
			{
				{"instagram.com/p/BGspzG0NiK_", true},
				{"instagram.com/p/BGUcLFrqiRU/", true},
				{"www.instagram.com/p/BGfbjpNQ9v2/", true},
				{"https://www.instagram.com/p/BGfbjpNQ9v2/", true},
				{"https://instagram.com/p/BGfbjpNQ9v2/", true},
				{"www.instagram,com/rzlganteng",false },
				{"www.instagram.com/rzlganteng",false },
				{"https://www.youtube.com/watch?v=KFOgIijPWGY", false },
				{"example.com/p/BGspzG0NiK_", false },
				{"www.example.com/p/BGspzG0NiK_", false },
				{"https://example.com/p/BGspzG0NiK_", false }
			};

			// Act
			var results = testCases.ToDictionary( kvp => kvp.Key, kvp => extractor.CanExtract( kvp.Key ) );

			// Assert
			foreach( var kvp in results )
			{
				Assert.AreEqual( testCases[kvp.Key], kvp.Value, kvp.Key );
			}
		}
	}
}