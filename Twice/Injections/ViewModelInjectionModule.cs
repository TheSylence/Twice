using GalaSoft.MvvmLight.Messaging;
using Ninject.Modules;
using Twice.ViewModels.Main;

namespace Twice.Injections
{
	internal class ViewModelInjectionModule : NinjectModule
	{
		/// <summary>
		/// Loads the module into the kernel.
		/// </summary>
		public override void Load()
		{
			Bind<IMessenger>().ToConstant( Messenger.Default );

			Bind<IMainViewModel>().To<MainViewModel>();
		}
	}
}