using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Ninject;
using Ninject.Modules;
using Twice.Injections;

namespace Twice
{
	/// <summary>
	/// Interaction logic for App.xaml
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
			Kernel = new StandardKernel( InjectionModules.ToArray() );

			base.OnStartup( e );
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