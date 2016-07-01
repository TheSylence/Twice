using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Twice.Models.Twitter;

namespace Twice.Tests.Models.Twitter
{
	[TestClass, ExcludeFromCodeCoverage]
	public class CharacterCountingTests
	{
		[TestMethod, TestCategory( "Models.Twitter" )]
		public void EmptyStringHasZeroLength()
		{
			// Arrange
			var str = string.Empty;

			// Act
			int count = TwitterHelper.CountCharacters( str, MockConfig() );

			// Assert
			Assert.AreEqual( 0, count );
		}

		[TestMethod, TestCategory( "Models.Twitter" )]
		public void InvalidTweet141Chars()
		{
			// Arrange
			const string str =
				"A lie gets halfway around the world before the truth has a chance to get its pants on. -- Winston Churchill (1874-1965) http://bit.ly/dJpywL";

			// Act
			int count = TwitterHelper.CountCharacters( str, MockConfig() );

			// Assert
			Assert.IsTrue( count > 140 );
		}

		[TestMethod, TestCategory( "Models.Twitter" )]
		public void InvalidTweet141CharsNewLine()
		{
			// Arrange
			const string str =
				"A lie gets halfway around the world before the truth has a chance to get its pants on. \n- Winston Churchill (1874-1965) http://bit.ly/dJpywL";

			// Act
			int count = TwitterHelper.CountCharacters( str, MockConfig() );

			// Assert
			Assert.IsTrue( count > 140 );
		}

		[TestMethod, TestCategory( "Models.Twitter" )]
		public void SimpleAsciiTextHasSimpleLength()
		{
			// Arrange
			var testCases = new Dictionary<int, string>();
			var rand = new Random();
			for( int i = 1; i < 140; ++i )
			{
				var sb = new StringBuilder();
				for( int n = 0; n < i; ++n )
				{
					bool upper = rand.Next( 0, 1 ) == 1;
					sb.Append( (char)( ( upper
						? 'A'
						: 'a' ) + rand.Next( 0, 27 ) ) );
				}

				testCases.Add( i, sb.ToString() );
			}

			// Act
			var results = testCases.ToDictionary( kvp => kvp.Key, kvp => TwitterHelper.CountCharacters( kvp.Value, MockConfig() ) );

			// Assert
			foreach( int l in testCases.Keys )
			{
				Assert.AreEqual( l, results[l] );
			}
		}

		[TestMethod, TestCategory( "Models.Twitter" )]
		public void TwitterConformance()
		{
			// Arrange
			var testCases = new Dictionary<string, int>
			{
				{"This is a test.", 15},
				{"http://test.com", 23},
				{"https://test.com", 23},
				{"test.com", 23},
				{"Test https://test.com test https://test.com test.com test", 86},

				// FIXME: This results in 4 :(
				//{"\U00010000\U0010ffff", 2},
				{"저찀쯿쿿", 4},
				{"H\U0001f431☺", 3}
			};

			// Act
			var results = testCases.ToDictionary( kvp => kvp.Key, kvp => TwitterHelper.CountCharacters( kvp.Key, MockConfig() ) );

			// Assert
			foreach( var str in testCases.Keys )
			{
				Assert.AreEqual( testCases[str], results[str], str );
			}
		}

		[TestMethod, TestCategory( "Models.Twitter" )]
		public void TwitterExampleIsCountedCorrectly()
		{
			// Arrange
			const string str = "café";

			// Act
			int count = TwitterHelper.CountCharacters( str, MockConfig() );

			// Assert
			Assert.AreEqual( 4, count );
		}

		[TestMethod, TestCategory( "Models.Twitter" )]
		public void ValidTweet140Chars()
		{
			// Arrange
			const string str =
				"A lie gets halfway around the world before the truth has a chance to get its pants on. Winston Churchill (1874-1965) http://bit.ly/dJpywL";

			// Act
			int count = TwitterHelper.CountCharacters( str, MockConfig() );

			// Assert
			Assert.AreEqual( 140, count );
		}

		[TestMethod, TestCategory( "Models.Twitter" )]
		public void ValidTweet140CharsDoubleByte()
		{
			// Arrange
			const string str =
				"のののののののののののののののののののののののののののののののののののののののののののののののののののののののののののののののののののののののののののののののののののののののののののののののののののののののののののののののののののののののののののののののののののののののののののの";

			// Act
			int count = TwitterHelper.CountCharacters( str, MockConfig() );

			// Assert
			Assert.AreEqual( 140, count );
		}

		[TestMethod, TestCategory( "Models.Twitter" )]
		public void ValidTweet140CharsDoubleWord()
		{
			// Arrange
			const string str =
				"\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431\U0001f431";

			// Act
			int count = TwitterHelper.CountCharacters( str, MockConfig() );

			// Assert
			Assert.AreEqual( 140, count );
		}

		[TestMethod, TestCategory( "Models.Twitter" )]
		public void ValidTweet140CharsWithAccent()
		{
			// Arrange
			const string str =
				"A lié géts halfway arøünd thé wørld béføré thé truth has a chance tø get its pants øn. Winston Churchill (1874-1965) http://bit.ly/dJpywL";

			// Act
			int count = TwitterHelper.CountCharacters( str, MockConfig() );

			// Assert
			Assert.AreEqual( 140, count );
		}

		[TestMethod, TestCategory( "Models.Twitter" )]
		public void ValidTweetLessThan20Chars()
		{
			// Arrange
			const string str = "I am a Tweet";

			// Act
			int count = TwitterHelper.CountCharacters( str, MockConfig() );

			// Assert
			Assert.AreEqual( 12, count );
		}

		private static ITwitterConfiguration MockConfig()
		{
			var cfg = new Mock<ITwitterConfiguration>();
			cfg.SetupGet( c => c.UrlLength ).Returns( 23 );
			cfg.SetupGet( c => c.UrlLengthHttps ).Returns( 23 );

			return cfg.Object;
		}
	}
}