using System.Diagnostics.CodeAnalysis;
using GalaSoft.MvvmLight.Messaging;
using Ninject.Modules;
using Twice.ViewModels;
using Twice.ViewModels.Accounts;
using Twice.ViewModels.ColumnManagement;
using Twice.ViewModels.Columns.Definitions;
using Twice.ViewModels.Dialogs;
using Twice.ViewModels.Info;
using Twice.ViewModels.Main;
using Twice.ViewModels.Profile;
using Twice.ViewModels.Settings;
using Twice.ViewModels.Twitter;

namespace Twice.Injections
{
	[ExcludeFromCodeCoverage]
	internal class ViewModelInjectionModule : NinjectModule
	{
		/// <summary>
		/// Loads the module into the kernel.
		/// </summary>
		public override void Load()
		{
			Bind<IMessenger>().ToConstant( Messenger.Default );

			Bind<IColumnDefinitionList>().To<ColumnDefinitionList>().InSingletonScope();

			Bind<IMainViewModel>().To<MainViewModel>();

			Bind<ISettingsDialogViewModel>().To<SettingsDialogViewModel>();
			Bind<IVisualSettings>().To<VisualSettings>();
			Bind<IGeneralSettings>().To<GeneralSettings>();
			Bind<IMuteSettings>().To<MuteSettings>();
			Bind<INotificationSettings>().To<NotificationSettings>();

			Bind<IComposeTweetViewModel>().To<ComposeTweetViewModel>();

			Bind<IProfileDialogViewModel>().To<ProfileDialogViewModel>();

			Bind<IInfoDialogViewModel>().To<InfoDialogViewModel>();

			Bind<IAccountsDialogViewModel>().To<AccountsDialogViewModel>();
			Bind<IAddColumnDialogViewModel>().To<AddColumnDialogViewModel>();
			Bind<IColumnTypeSelectionDialogViewModel>().To<ColumnTypeSelectionDialogViewModel>();

			Bind<ITextInputDialogViewModel>().To<TextInputDialogViewModel>();

			Bind<INotifier>().To<Notifier>();
		}
	}
}