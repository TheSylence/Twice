using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Diagnostics.CodeAnalysis;
using Twice.Models.Twitter;
using Twice.ViewModels.Twitter;

namespace Twice.Tests.ViewModels.Twitter
{
	[TestClass, ExcludeFromCodeCoverage]
	public class ColumnItemTests
	{
		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void RelativeTimeCanBeUpdated()
		{
			// Arrange
			var context = new Mock<IContextEntry>();
			var status = new StatusViewModel( DummyGenerator.CreateDummyStatus(), context.Object, null, null );

			bool raised = false;
			status.PropertyChanged += ( s, e ) =>
			{
				if( e.PropertyName == nameof( StatusViewModel.CreatedAt ) )
				{
					raised = true;
				}
			};

			// Act
			status.UpdateRelativeTime();

			// Assert
			Assert.IsTrue( raised );
		}
	}
}