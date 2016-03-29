using System.Collections.Generic;
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
using Twice.Injections;
using Twice.Models.Configuration;
using WPFLocalizeExtension.Engine;

namespace Twice
{
	/// <summary>
	///     Interaction logic for App.xaml
	/// </summary>
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
			Kernel = new StandardKernel( InjectionModules.ToArray() );

			base.OnStartup( e );

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

			LocalizeDictionary dict = LocalizeDictionary.Instance;
			dict.IncludeInvariantCulture = false;
			dict.SetCurrentThreadCulture = true;
			dict.Culture = CultureInfo.GetCultureInfo( conf.General.Language );

			var xmlLang = XmlLanguage.GetLanguage( dict.Culture.IetfLanguageTag );
			FrameworkElement.LanguageProperty.OverrideMetadata( typeof( FrameworkElement ), new FrameworkPropertyMetadata( xmlLang ) );
			FrameworkElement.LanguageProperty.OverrideMetadata( typeof( Run ), new FrameworkPropertyMetadata( xmlLang ) );
		}

		public static IKernel Kernel { get; private set; }

		private static IEnumerable<INinjectModule> InjectionModules
		{
			get
			{
				yield return new ModelInjectionModule();
				yield return new ViewModelInjectionModule();
				yield return new ServiceInjectionModule();
			}
		}
	}
}