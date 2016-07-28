using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Twice.ViewModels.Info;

namespace Twice.Tests.ViewModels.Info
{
	[TestClass, ExcludeFromCodeCoverage]
	public class InfoDialogViewModelTests
	{
		[TestMethod, TestCategory( "ViewModels.Info" )]
		public void ChangelogIsCorrectlyLoaded()
		{
			// Arrange Act
			var vm = new InfoDialogViewModel();

			// Assert
			Assert.IsTrue( vm.Changelogs.Any() );
		}

		[TestMethod, TestCategory( "ViewModels.Info" )]
		public void LicenseInformationIsCorrectlyLoaded()
		{
			// Arrange Act
			var vm = new InfoDialogViewModel();

			// Assert
			Assert.IsTrue( vm.Licenses.Any() );
		}
	}
}