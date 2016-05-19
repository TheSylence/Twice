using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twice.Models.Configuration;
using Twice.Utilities;

namespace Twice.Tests.Models.Configuration
{
	[TestClass, ExcludeFromCodeCoverage]
	public class ConfigTests
	{
		[TestMethod, TestCategory( "Models.Configuration" )]
		public void LoadingConfigWithTrashDoesNotCrash()
		{
			// Arrange
			var fileName = Path.GetTempFileName();
			File.WriteAllText( fileName, @"Hello world this is a test" );

			// Act ReSharper disable once ObjectCreationAsStatement
			// ReSharper disable once ObjectCreationAsStatement
			var ex = ExceptionAssert.Catch<Exception>( () => new Config( fileName, new Serializer() ) );

			// Assert
			Assert.IsNull( ex, ex?.ToString() );
		}

		[TestMethod, TestCategory( "Models.Configuration" )]
		public void SavedConfigCanBeLoaded()
		{
			// Arrange
			var fileName = Path.GetTempFileName();
			var config = new Config( fileName, new Serializer() )
			{
				General =
				{
					CheckForUpdates = false,
					IncludePrereleaseUpdates = true,
					Language = "test",
					RealtimeStreaming = false
				},
				Visual =
				{
					AccentColor = "a",
					FontSize = 1,
					HashtagColor = "b",
					InlineMedia = false,
					LinkColor = "c",
					MentionColor = "d",
					PrimaryColor = "e",
					UseDarkTheme = true
				},
				Notifications =
				{
					PopupDisplayCorner = Corner.BottomRight,
					PopupDisplay = "test",
					PopupEnabled = true,
					SoundEnabled = false,
					SoundFileName = "file.name",
					ToastsEnabled = true
				}
			};

			config.Mute.Entries.Add( new MuteEntry {Filter = "test"} );
			config.Mute.Entries.Add( new MuteEntry {Filter = "@one"} );
			config.Mute.Entries.Add( new MuteEntry {Filter = "#two"} );
			config.Mute.Entries.Add( new MuteEntry {Filter = ":three"} );
			config.Mute.Entries.Add( new MuteEntry {Filter = "abc", EndDate = DateTime.MaxValue} );

			// Act
			config.Save();
			var cfg = new Config( fileName, new Serializer() );

			// Assert
			Assert.AreEqual( config.General.CheckForUpdates, cfg.General.CheckForUpdates );
			Assert.AreEqual( config.General.IncludePrereleaseUpdates, cfg.General.IncludePrereleaseUpdates );
			Assert.AreEqual( config.General.Language, cfg.General.Language );
			Assert.AreEqual( config.General.RealtimeStreaming, cfg.General.RealtimeStreaming );

			Assert.AreEqual( config.Visual.AccentColor, cfg.Visual.AccentColor );
			Assert.AreEqual( config.Visual.FontSize, cfg.Visual.FontSize );
			Assert.AreEqual( config.Visual.HashtagColor, cfg.Visual.HashtagColor );
			Assert.AreEqual( config.Visual.LinkColor, cfg.Visual.LinkColor );
			Assert.AreEqual( config.Visual.MentionColor, cfg.Visual.MentionColor );
			Assert.AreEqual( config.Visual.PrimaryColor, cfg.Visual.PrimaryColor );
			Assert.AreEqual( config.Visual.UseDarkTheme, cfg.Visual.UseDarkTheme );

			Assert.AreEqual( config.Notifications.PopupDisplayCorner, cfg.Notifications.PopupDisplayCorner );
			Assert.AreEqual( config.Notifications.PopupDisplay, cfg.Notifications.PopupDisplay );
			Assert.AreEqual( config.Notifications.PopupEnabled, cfg.Notifications.PopupEnabled );
			Assert.AreEqual( config.Notifications.SoundEnabled, cfg.Notifications.SoundEnabled );
			Assert.AreEqual( config.Notifications.SoundFileName, cfg.Notifications.SoundFileName );
			Assert.AreEqual( config.Notifications.ToastsEnabled, cfg.Notifications.ToastsEnabled );

			Assert.IsNotNull( cfg.Mute.Entries.SingleOrDefault( e => e.Filter == "test" ) );
			Assert.IsNotNull( cfg.Mute.Entries.SingleOrDefault( e => e.Filter == "@one" ) );
			Assert.IsNotNull( cfg.Mute.Entries.SingleOrDefault( e => e.Filter == "#two" ) );
			Assert.IsNotNull( cfg.Mute.Entries.SingleOrDefault( e => e.Filter == ":three" ) );
			Assert.IsNotNull( cfg.Mute.Entries.SingleOrDefault( e => e.Filter == "abc" && e.EndDate == DateTime.MaxValue ) );
		}
	}
}