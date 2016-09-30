using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Twice.Models.Media;
using Twice.ViewModels.Twitter;

namespace Twice.Tests.ViewModels.Twitter
{
	[TestClass]
	public class CardViewModelTests
	{
		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void EmptyHostDoesNotCrash()
		{
			// Arrange
			var card = new TwitterCard( new Dictionary<string, string>() );
			var vm = new CardViewModel( card );

			// Act
			string url = null;
			var ex = ExceptionAssert.Catch<NullReferenceException>( () => url = vm.DisplayUrl );

			// Assert
			Assert.IsNull( ex );
			Assert.AreEqual( string.Empty, url );
		}
	}
}