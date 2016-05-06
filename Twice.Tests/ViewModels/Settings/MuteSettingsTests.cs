using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twice.Models.Configuration;
using Twice.ViewModels.Settings;

namespace Twice.Tests.ViewModels.Settings
{
	[TestClass]
	public class MuteSettingsTests
	{
		[TestMethod, TestCategory( "ViewModels.Settings" )]
		public void AddCanBeCancelled()
		{
			// Arrange
			var config = new Mock<IConfig>();
			config.SetupGet( c => c.Mute ).Returns( new MuteConfig() );
			var vm = new MuteSettings( config.Object );
			vm.AddCommand.Execute( null );

			// Act
			vm.EditData.CancelCommand.Execute( null );

			// Assert
			Assert.IsNull( vm.EditData );
		}

		[TestMethod, TestCategory( "ViewModels.Settings" )]
		public void CorrectDataIsLoaded()
		{
			// Arrange
			var muteCfg = new MuteConfig();
			muteCfg.Entries.Add( new MuteEntry {Filter = "test", EndDate = DateTime.Now} );

			var config = new Mock<IConfig>();
			config.SetupGet( c => c.Mute ).Returns( muteCfg );

			// Act
			var vm = new MuteSettings( config.Object );

			// Assert
			Assert.AreEqual( 1, vm.Entries.Count );
			Assert.AreEqual( "test", vm.Entries.First().Filter );
			Assert.IsTrue( vm.Entries.First().HasEndDate );
			Assert.AreNotEqual( string.Empty, vm.HelpDocument );
		}

		[TestMethod, TestCategory( "ViewModels.Settings" )]
		public void EditCommandsNeedsSelection()
		{
			// Arrange
			var muteCfg = new MuteConfig();
			muteCfg.Entries.Add( new MuteEntry {Filter = "test", EndDate = DateTime.Now} );

			var config = new Mock<IConfig>();
			config.SetupGet( c => c.Mute ).Returns( muteCfg );

			var vm = new MuteSettings( config.Object );

			// Act
			bool withoutSelection = vm.EditCommand.CanExecute( null );
			vm.SelectedEntry = vm.Entries.First();
			bool withSelection = vm.EditCommand.CanExecute( null );

			// Assert
			Assert.IsFalse( withoutSelection );
			Assert.IsTrue( withSelection );
		}

		[TestMethod, TestCategory( "ViewModels.Settings" )]
		public void RemoveCommandsNeedsSelection()
		{
			// Arrange
			var muteCfg = new MuteConfig();
			muteCfg.Entries.Add( new MuteEntry {Filter = "test", EndDate = DateTime.Now} );

			var config = new Mock<IConfig>();
			config.SetupGet( c => c.Mute ).Returns( muteCfg );

			var vm = new MuteSettings( config.Object );

			// Act
			bool withoutSelection = vm.RemoveCommand.CanExecute( null );
			vm.SelectedEntry = vm.Entries.First();
			bool withSelection = vm.RemoveCommand.CanExecute( null );

			// Assert
			Assert.IsFalse( withoutSelection );
			Assert.IsTrue( withSelection );
		}

		[TestMethod, TestCategory( "ViewModels.Settings" )]
		public void SaveWritesCorrectValues()
		{
			// Arrange

			var muteCfg = new MuteConfig();
			muteCfg.Entries.Add( new MuteEntry {Filter = "test", EndDate = DateTime.Now} );

			var config = new Mock<IConfig>();
			config.SetupGet( c => c.Mute ).Returns( muteCfg );

			var vm = new MuteSettings( config.Object );

			// Act
			vm.Entries.Clear();
			vm.Entries.Add( new MuteEntry {Filter = "filter", EndDate = new DateTime( 2020, 1, 2 )} );
			vm.SaveTo( config.Object );

			// Assert
			Assert.AreEqual( 1, muteCfg.Entries.Count );
			Assert.AreEqual( "filter", muteCfg.Entries[0].Filter );
			Assert.AreEqual( new DateTime( 2020, 1, 2 ), muteCfg.Entries[0].EndDate );
		}
	}
}