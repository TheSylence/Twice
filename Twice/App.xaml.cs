using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using GalaSoft.MvvmLight.Threading;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using Ninject;
using Ninject.Modules;
using NLog;
using NLog.Config;
using NLog.Targets;
using Twice.Injections;
using Twice.Models.Configuration;
using WPFLocalizeExtension.Engine;

namespace Twice
{
	/// <summary>
	///     Interaction logic for App.xaml
	/// </summary>
	[ExcludeFromCodeCoverage]
	public partial class App
	{
		protected override void OnExit( ExitEventArgs e )
		{
			Kernel.Dispose();

			base.OnExit( e );
		}

		protected override void OnStartup( StartupEventArgs e )
		{
			DispatcherHelper.Initialize();
			Kernel = new Kernel( );

			base.OnStartup( e );
			ConfigureLogging();

			var conf = Kernel.Get<IConfig>();
			var palette = new PaletteHelper();
			palette.SetLightDark( conf.Visual.UseDarkTheme );
			palette.ReplaceAccentColor( conf.Visual.AccentColor );
			palette.ReplacePrimaryColor( conf.Visual.PrimaryColor );

			var swatches = new SwatchesProvider().Swatches.ToArray();

			var resDict = new ResourceDictionary();
			resDict.BeginInit();
			{
				resDict.Add( "HashtagBrush", new SolidColorBrush( swatches.First( s => s.Name == conf.Visual.HashtagColor ).ExemplarHue.Color ) );
				resDict.Add( "LinkBrush", new SolidColorBrush( swatches.First( s => s.Name == conf.Visual.LinkColor ).ExemplarHue.Color ) );
				resDict.Add( "MentionBrush", new SolidColorBrush( swatches.First( s => s.Name == conf.Visual.MentionColor ).ExemplarHue.Color ) );
				resDict.Add( "GlobalFontSize", (double)conf.Visual.FontSize );
			}
			resDict.EndInit();

			Resources.MergedDictionaries.Add( resDict );

			ChangeLanguage( conf.General.Language );
		}

		private static void ChangeLanguage( string language )
		{
			LocalizeDictionary dict = LocalizeDictionary.Instance;
			dict.IncludeInvariantCulture = true;
			dict.SetCurrentThreadCulture = true;
			dict.Culture = CultureInfo.GetCultureInfo( language );

			var xmlLang = XmlLanguage.GetLanguage( dict.Culture.IetfLanguageTag );
			FrameworkElement.LanguageProperty.OverrideMetadata( typeof( FrameworkElement ), new FrameworkPropertyMetadata( xmlLang ) );
			FrameworkElement.LanguageProperty.OverrideMetadata( typeof( Run ), new FrameworkPropertyMetadata( xmlLang ) );
		}

		private void ConfigureLogging()
		{
			var config = new LoggingConfiguration();

			var debuggerTarget = new DebuggerTarget
			{
				Layout = "[${level:uppercase=true}] ${logger}: ${message} ${exception}"
			};
			config.AddTarget( "debugger", debuggerTarget );

			var fileTarget = new FileTarget
			{
				Layout = "${longdate} [${level:uppercase=true}] ${logger}: ${message} ${exception}",
				FileName = "log.txt",
				DeleteOldFileOnStartup = true
			};
			config.AddTarget( "Logfile", fileTarget );

			var debuggerRule = new LoggingRule( "*", LogLevel.Trace, debuggerTarget );
			config.LoggingRules.Add( debuggerRule );

			var fileRule = new LoggingRule( "*", LogLevel.Trace, fileTarget );
			config.LoggingRules.Add( fileRule );

			LogManager.Configuration = config;
		}

		public static IKernel Kernel { get; private set; }

		
	}
}