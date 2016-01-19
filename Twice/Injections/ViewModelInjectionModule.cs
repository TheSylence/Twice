using GalaSoft.MvvmLight.Messaging;
using Ninject.Modules;
using Twice.ViewModels.Main;
using Twice.ViewModels.Profile;
using Twice.ViewModels.Settings;
using Twice.ViewModels.Twitter;

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

			Bind<ISettingsDialogViewModel>().To<SettingsDialogViewModel>();
			Bind<IVisualSettings>().To<VisualSettings>();
			Bind<IGeneralSettings>().To<GeneralSettings>();
			Bind<IMuteSettings>().To<MuteSettings>();

			Bind<IComposeTweetViewModel>().To<ComposeTweetViewModel>();

			Bind<IProfileDialogViewModel>().To<ProfileDialogViewModel>();
		}
	}
}