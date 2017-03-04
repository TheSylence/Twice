using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
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
using Twice.Injections;
using Twice.Models.Configuration;
using Twice.Models.Proxy;
using Twice.Models.Scheduling;
using Twice.Models.Twitter;
using Twice.Resources;
using Twice.Utilities.Os;
using Twice.Utilities.Ui;
using Twice.Views;
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
			Kernel = new Kernel();

			Task.Run( () =>
			{
				Settings.UIMode = UIMode.Full;
				Settings.MiniDumpType = MiniDumpType.Normal;
				Settings.StoragePath = StoragePath.CurrentDirectory;
				Settings.UIProvider = UIProvider.WPF;
				Settings.AdditionalReportFiles.Add( "log*.txt" );
				Settings.AdditionalReportFiles.Add( Constants.IO.ConfigFileName );
				Settings.SleepBeforeSend = 20;
				Settings.StopReportingAfter = 100;
				Settings.AddDestinationFromConnectionString( $"Type=Http;Url={Constants.Web.CrashReportUrl};" );

				Settings.ReleaseMode = true;
			} ).Forget();

			AppDomain.CurrentDomain.UnhandledException += Handler.UnhandledException;
			Current.DispatcherUnhandledException += Handler.DispatcherUnhandledException;

			ProxyServer = new MediaProxyServer();

			Timeline.DesiredFrameRateProperty.OverrideMetadata( typeof( Timeline ), new FrameworkPropertyMetadata {DefaultValue = 30} );
		}

		internal static void ApplyWindowSettings( Window window )
		{
			WindowSettings = WindowSettings.Load( Constants.IO.WindowSettingsFileName );
			WindowSettings?.Apply( new WindowWrapper( window ) );

			// Default settings contain no width or height so instanciate them only AFTER setting the
			// position otherwise the window would disappear
			if( WindowSettings == null )
			{
				WindowSettings = new WindowSettings();
			}
		}

		protected override void OnExit( ExitEventArgs e )
		{
			Scheduler?.Stop();
			ProxyServer.Stop();
			SingleInstance.Stop();

			CentralHandler?.Dispose();
			LogTo.Info( "Application exit" );
			Kernel?.Dispose();

			base.OnExit( e );
		}

		protected override void OnStartup( StartupEventArgs e )
		{
			// Ensure that only one instance of the application can run at any time
			if( !SingleInstance.Start() )
			{
				SingleInstance.ShowFirstInstance();
				Shutdown( 0 );
				return;
			}

			ConfigureLogging();
			LogTo.Info( "Application start" );
			LogEnvironmentInfo();

			DispatcherHelper.Initialize();
			ProxyServer.Start( Kernel.Get<ITwitterContextList>() );

			Scheduler = Kernel.Get<IScheduler>();
			Scheduler.Start();

			base.OnStartup( e );

			CentralHandler = new CentralMessageHandler( Kernel );

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
			ChangeLanguage( conf );
		}

		internal static void SaveWindowSettings( Window window )
		{
			WindowSettings.Save( new WindowWrapper( window ) );
		}

		private static void ChangeLanguage( IConfig config )
		{
			var language = config.General.Language;

			LocalizeDictionary dict = LocalizeDictionary.Instance;
			dict.IncludeInvariantCulture = true;
			dict.SetCurrentThreadCulture = true;

			if( string.IsNullOrWhiteSpace( language ) )
			{
				// User has not decided for a language yet. Try to use current UI language
				LogTo.Info( $"User has not set language. Trying to use {CultureInfo.CurrentUICulture}" );

				string localizedCode = Strings.ResourceManager.GetString( "__Language_Code__", CultureInfo.CurrentUICulture );

				var culture = CultureInfo.CreateSpecificCulture( localizedCode );

				if( !CultureInfo.InvariantCulture.Equals( culture ) )
				{
					dict.Culture = culture;
					config.General.Language = culture.Name;
					config.Save();
				}
			}
			else
			{
				dict.Culture = CultureInfo.GetCultureInfo( language );
			}

			// This is done so DateTime's in XAML are parsed based on the user language instead of
			// en-US which is the default language for XAML.
			var xmlLang = XmlLanguage.GetLanguage( dict.Culture.IetfLanguageTag );
			FrameworkElement.LanguageProperty.OverrideMetadata( typeof( FrameworkElement ),
				new FrameworkPropertyMetadata( xmlLang ) );
			FrameworkElement.LanguageProperty.OverrideMetadata( typeof( Run ), new FrameworkPropertyMetadata( xmlLang ) );
		}

		private void ConfigureLogging()
		{
			var config = new LoggingConfiguration();

			var fileTarget = new FileTarget
			{
				Layout = "${longdate} [${level:uppercase=true}] ${logger}: ${message} ${exception}",
				FileName = "log.txt",
				DeleteOldFileOnStartup = true
			};
			config.AddTarget( "Logfile", fileTarget );

			var fileRule = new LoggingRule( "*", LogLevel.Trace, fileTarget );
			config.LoggingRules.Add( fileRule );

			// Don't bother with logging something to a debugger in release mode
			if( Constants.Debug )
			{
				var debuggerTarget = new DebuggerTarget
				{
					Layout = "[${level:uppercase=true}] ${logger}: ${message} ${exception}"
				};
				config.AddTarget( "debugger", debuggerTarget );

				var debuggerRule = new LoggingRule( "*", LogLevel.Trace, debuggerTarget );
				config.LoggingRules.Add( debuggerRule );
			}

			LogManager.Configuration = config;
		}

		private static void LogEnvironmentInfo()
		{
			LogTo.Info( $"App version: {Assembly.GetExecutingAssembly().GetName().Version}" );

			string osVersionString = OsVersionInfo.OsBits == OsVersionInfo.SoftwareArchitecture.Bit64
				? $"{OsVersionInfo.Name} {OsVersionInfo.Edition} 64bit"
				: $"{OsVersionInfo.Name} {OsVersionInfo.Edition}";

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

		private static WindowSettings WindowSettings;

		public static IKernel Kernel { get; private set; }
		private readonly MediaProxyServer ProxyServer;
		private CentralMessageHandler CentralHandler;
		private IScheduler Scheduler;
	}
}