using Anotar.NLog;
using GalaSoft.MvvmLight.Threading;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using NBug;
using NBug.Enums;
using Ninject;
using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using Twice.Injections;
using Twice.Models.Configuration;
using Twice.Utilities.Os;
using WPFLocalizeExtension.Engine;

namespace Twice
{
	/// <summary>
	///     Interaction logic for App.xaml
	/// </summary>
	[ExcludeFromCodeCoverage]
	public partial class App
	{
		public App()
		{
			Settings.UIMode = UIMode.Full;
			Settings.MiniDumpType = MiniDumpType.Tiny;
			Settings.StoragePath = StoragePath.CurrentDirectory;
			Settings.UIProvider = UIProvider.WPF;
			Settings.AdditionalReportFiles.Add( "log*.txt" );
			Settings.AdditionalReportFiles.Add( Constants.IO.ConfigFileName );
			Settings.SleepBeforeSend = 20;
			Settings.StopReportingAfter = 100;
			Settings.AddDestinationFromConnectionString( "Type=Http;Url=http://software.btbsoft.org/twice/crash/report.php;" );

			Settings.ReleaseMode = true;

			AppDomain.CurrentDomain.UnhandledException += Handler.UnhandledException;
			Current.DispatcherUnhandledException += Handler.DispatcherUnhandledException;
		}

		protected override void OnExit( ExitEventArgs e )
		{
			SingleInstance.Stop();

			LogTo.Info( "Application exit" );
			Kernel?.Dispose();

			base.OnExit( e );
		}

		protected override void OnStartup( StartupEventArgs e )
		{
			if( !SingleInstance.Start() )
			{
				SingleInstance.ShowFirstInstance();
				Shutdown( 0 );
				return;
			}

			DispatcherHelper.Initialize();
			Kernel = new Kernel();

			base.OnStartup( e );
			ConfigureLogging();
			LogTo.Info( "Application start" );
			LogEnvironmentInfo();

			var conf = Kernel.Get<IConfig>();
			var palette = new PaletteHelper();
			palette.SetLightDark( conf.Visual.UseDarkTheme );
			palette.ReplaceAccentColor( conf.Visual.AccentColor );
			palette.ReplacePrimaryColor( conf.Visual.PrimaryColor );

			var swatches = new SwatchesProvider().Swatches.ToArray();

			var resDict = new ResourceDictionary();
			resDict.BeginInit();
			{
				resDict.Add( "HashtagBrush",
					new SolidColorBrush( swatches.First( s => s.Name == conf.Visual.HashtagColor ).ExemplarHue.Color ) );
				resDict.Add( "LinkBrush",
					new SolidColorBrush( swatches.First( s => s.Name == conf.Visual.LinkColor ).ExemplarHue.Color ) );
				resDict.Add( "MentionBrush",
					new SolidColorBrush( swatches.First( s => s.Name == conf.Visual.MentionColor ).ExemplarHue.Color ) );
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
			FrameworkElement.LanguageProperty.OverrideMetadata( typeof(FrameworkElement),
				new FrameworkPropertyMetadata( xmlLang ) );
			FrameworkElement.LanguageProperty.OverrideMetadata( typeof(Run), new FrameworkPropertyMetadata( xmlLang ) );
		}

		private static void LogEnvironmentInfo()
		{
			LogTo.Info( $"App version: {Assembly.GetExecutingAssembly().GetName().Version}" );

			string osVersionString;
			if( OsVersionInfo.OsBits == OsVersionInfo.SoftwareArchitecture.Bit64 )
			{
				osVersionString = $"{OsVersionInfo.Name} {OsVersionInfo.Edition} 64bit";
			}
			else
			{
				osVersionString = $"{OsVersionInfo.Name} {OsVersionInfo.Edition}";
			}

			LogTo.Info( osVersionString );
			if( !string.IsNullOrEmpty( OsVersionInfo.ServicePack ) )
			{
				LogTo.Info( $"Service Pack {OsVersionInfo.ServicePack}" );
			}
			LogTo.Info( $"Version {OsVersionInfo.Version}" );
			if( OsVersionInfo.ProcessorBits == OsVersionInfo.ProcessorArchitecture.Bit64 )
			{
				LogTo.Info( "64bit CPU" );
			}
			if( OsVersionInfo.ProgramBits == OsVersionInfo.SoftwareArchitecture.Bit64 )
			{
				LogTo.Info( "64bit app" );
			}
			LogTo.Info( $"CLR version: {Environment.Version}" );
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