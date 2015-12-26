using GalaSoft.MvvmLight.Threading;
using Ninject;
using Ninject.Modules;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using MahApps.Metro;
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
			Resources["HashtagBrush"] = ThemeManager.GetAccent( conf.Visual.HashtagColor ).Resources["HightlightBrush"];
			Resources["LinkBrush"] = ThemeManager.GetAccent( conf.Visual.LinkColor ).Resources["HightlightBrush"];
			Resources["MentionBrush"] = ThemeManager.GetAccent( conf.Visual.MentionColor ).Resources["HightlightBrush"];
			Resources["GlobalFontSize"] = (double)conf.Visual.FontSize;

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