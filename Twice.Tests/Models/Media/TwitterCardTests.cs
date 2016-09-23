using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Twice.Models.Media;

namespace Twice.Tests.Models.Media
{
	[TestClass, ExcludeFromCodeCoverage]
	public class TwitterCardTests
	{
		[TestMethod, TestCategory( "Models.Media" )]
		public void CardWithoutDataIsInvalid()
		{
			// Arrange
			var meta = new Dictionary<string, string>();

			// Act
			var card = new TwitterCard( meta );

			// Assert
			Assert.IsFalse( card.IsValid );
		}

		[TestMethod, TestCategory( "Models.Media" )]
		public void DescriptionIsRequired()
		{
			// Arrange
			var meta = new Dictionary<string, string>
			{
				{"twitter:card", "summary"},
				{"twitter:title", "title"},
				{"twitter:site", "site"}
			};

			var metaFull = new Dictionary<string, string>
			{
				{"twitter:card", "summary"},
				{"twitter:site", "site"},
				{"twitter:title", "title"},
				{"twitter:description", "desc"}
			};

			// Act
			var card = new TwitterCard( meta );
			var validCard = new TwitterCard( metaFull );

			// Assert
			Assert.IsFalse( card.IsValid );
			Assert.IsTrue( validCard.IsValid );
		}

		[TestMethod, TestCategory( "Models.Media" )]
		public void SiteIsRequired()
		{
			// Arrange
			var meta = new Dictionary<string, string>
			{
				{"twitter:card", "summary"},
				{"twitter:title", "title"},
				{"twitter:description", "desc"}
			};

			var metaFull = new Dictionary<string, string>
			{
				{"twitter:card", "summary"},
				{"twitter:title", "title"},
				{"twitter:description", "desc"},
				{"twitter:site", "site"}
			};

			// Act
			var card = new TwitterCard( meta );
			var validCard = new TwitterCard( metaFull );

			// Assert
			Assert.IsFalse( card.IsValid );
			Assert.IsTrue( validCard.IsValid );
		}

		[TestMethod, TestCategory( "Models.Media" )]
		public void TitleIsRequired()
		{
			// Arrange
			var meta = new Dictionary<string, string>
			{
				{"twitter:card", "summary"},
				{"twitter:description", "desc"},
				{"twitter:site", "site"}
			};

			var metaFull = new Dictionary<string, string>
			{
				{"twitter:card", "summary"},
				{"twitter:description", "desc"},
				{"twitter:site", "site"},
				{"twitter:title", "title"}
			};

			// Act
			var card = new TwitterCard( meta );
			var validCard = new TwitterCard( metaFull );

			// Assert
			Assert.IsFalse( card.IsValid );
			Assert.IsTrue( validCard.IsValid );
		}

		[TestMethod, TestCategory( "Models.Media" )]
		public void TypeIsRequired()
		{
			// Arrange
			var meta = new Dictionary<string, string>
			{
				{"twitter:title", "title"},
				{"twitter:description", "desc"},
				{"twitter:site", "site"}
			};

			var metaFull = new Dictionary<string, string>
			{
				{"twitter:site", "site"},
				{"twitter:title", "title"},
				{"twitter:description", "desc"},
				{"twitter:card", "summary"}
			};

			// Act
			var card = new TwitterCard( meta );
			var validCard = new TwitterCard( metaFull );

			// Assert
			Assert.IsFalse( card.IsValid );
			Assert.IsTrue( validCard.IsValid );
		}

		[TestMethod, TestCategory( "Models.Media" )]
		public void UnknownTypeIsInvalid()
		{
			// Arrange
			var meta = new Dictionary<string, string>
			{
				{"twitter:site", "site"},
				{"twitter:title", "title"},
				{"twitter:description", "desc"},
				{"twitter:card", "summary_123"}
			};

			// Act
			var card = new TwitterCard( meta );

			// Assert
			Assert.IsFalse( card.IsValid );
		}
	}
}