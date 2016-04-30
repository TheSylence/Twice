using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Reflection;
using Twice.ViewModels.Twitter;

namespace Twice.Tests.ViewModels.Twitter
{
	[TestClass]
	public class MediaItemTests
	{
		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void EmptyDataDoesNotLoadImage()
		{
			// Arrange
			var media = new MediaItem( 123, new byte[0] );

			// Act
			var loaded = media.Image.Value;

			// Assert
			Assert.IsNull( loaded );
		}

		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void ImageCanBeLoaded()
		{
			// Arrange
			using( var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream( "Twice.Tests.Data.Image.png" ) )
			using( var mem = new MemoryStream() )
			{
				stream?.CopyTo( mem );

				var media = new MediaItem( 123, mem.ToArray() );

				// Act
				var loaded = media.Image.Value;

				// Assert
				Assert.IsNotNull( loaded );
				Assert.IsTrue( loaded.Width > 0 );
				Assert.IsTrue( loaded.Height > 0 );
			}
		}

		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void NullDataDoesNotLoadImage()
		{
			// Arrange
			var media = new MediaItem( 123, null );

			// Act
			var loaded = media.Image.Value;

			// Assert
			Assert.IsNull( loaded );
		}
	}
}