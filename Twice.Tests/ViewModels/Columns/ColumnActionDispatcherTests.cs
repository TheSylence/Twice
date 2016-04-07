using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twice.ViewModels.Columns;

namespace Twice.Tests.ViewModels.Columns
{
	[TestClass]
	public class ColumnActionDispatcherTests
	{
		[TestMethod, TestCategory( "ViewModels.Columns" )]
		public void BottomReachIsDispatched()
		{
			// Arrange
			var dispatcher = new ColumnActionDispatcher();

			bool bottom = false;
			dispatcher.BottomReached += ( s, e ) => bottom = true;

			// Act
			dispatcher.OnBottomReached();

			// Assert
			Assert.IsTrue( bottom );
		}

		[TestMethod, TestCategory( "ViewModels.Columns" )]
		public void HeaderClickIsDispatched()
		{
			// Arrange
			var dispatcher = new ColumnActionDispatcher();
			bool header = false;
			dispatcher.HeaderClicked += ( s, e ) => header = true;

			// Act
			dispatcher.OnHeaderClicked();

			// Assert
			Assert.IsTrue( header );
		}
	}
}