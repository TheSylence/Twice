using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using GalaSoft.MvvmLight.Threading;
using MahApps.Metro;
using Ninject;
using Ninject.Modules;
using Twice.Injections;
using Twice.Models.Configuration;
using WPFLocalizeExtension.Engine;

namespace Twice
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App
	{
		protected override void OnExit( ExitEventArgs e )
		{
			DispatcherHelper.Initialize();
			Kernel.Dispose();

			base.OnExit( e );
		}

		protected override void OnStartup( StartupEventArgs e )
		{
			Kernel = new StandardKernel( InjectionModules.ToArray() );

			base.OnStartup( e );

			var conf = Kernel.Get<IConfig>();
			ThemeManager.ChangeAppStyle( this, ThemeManager.GetAccent( conf.Visual.AccentName ), ThemeManager.GetAppTheme( conf.Visual.ThemeName ) );

			var resDict = new ResourceDictionary();
			resDict.BeginInit();
			{
				resDict.Add( "HashtagBrush", ThemeManager.GetAccent( conf.Visual.HashtagColor ).Resources["HighlightBrush"] );
				resDict.Add( "LinkBrush", ThemeManager.GetAccent( conf.Visual.LinkColor ).Resources["HighlightBrush"] );
				resDict.Add( "MentionBrush", ThemeManager.GetAccent( conf.Visual.MentionColor ).Resources["HighlightBrush"] );
				resDict.Add( "GlobalFontSize", (double)conf.Visual.FontSize );
			}
			resDict.EndInit();

			Resources.MergedDictionaries.Add( resDict );

			var test = TryFindResource("MentionBrush");

			LocalizeDictionary dict = LocalizeDictionary.Instance;
			dict.IncludeInvariantCulture = false;
			dict.SetCurrentThreadCulture = true;
			dict.Culture = CultureInfo.GetCultureInfo( conf.General.Language );

			FrameworkElement.LanguageProperty.OverrideMetadata( typeof( FrameworkElement ), new FrameworkPropertyMetadata( XmlLanguage.GetLanguage( dict.Culture.IetfLanguageTag ) ) );
			FrameworkElement.LanguageProperty.OverrideMetadata( typeof( Run ), new FrameworkPropertyMetadata( XmlLanguage.GetLanguage( dict.Culture.IetfLanguageTag ) ) );
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