using System.Diagnostics.CodeAnalysis;
using GalaSoft.MvvmLight.Messaging;
using Ninject.Modules;
using Twice.ViewModels;
using Twice.ViewModels.Accounts;
using Twice.ViewModels.ColumnManagement;
using Twice.ViewModels.Columns;
using Twice.ViewModels.Dialogs;
using Twice.ViewModels.Dialogs.Data;
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
		///     Loads the module into the kernel.
		/// </summary>
		public override void Load()
		{
			Bind<IAccountsDialogViewModel>().To<AccountsDialogViewModel>();
			Bind<IAddColumnDialogViewModel>().To<AddColumnDialogViewModel>();
			Bind<IColumnFactory>().To<ColumnFactory>();
			Bind<IColumnTypeSelectionDialogViewModel>().To<ColumnTypeSelectionDialogViewModel>();
			Bind<IComposeMessageViewModel>().To<ComposeMessageViewModel>();
			Bind<IComposeTweetViewModel>().To<ComposeTweetViewModel>();
			Bind<IConfirmDialogViewModel>().To<ConfirmDialogViewModel>();
			Bind<IDialogHostViewModel>().To<DialogHostViewModel>();
			Bind<IDialogStack>().To<DialogStack>().InSingletonScope();
			Bind<IGeneralSettings>().To<GeneralSettings>();
			Bind<IImageDialogViewModel>().To<ImageDialogViewModel>();
			Bind<IInfoDialogViewModel>().To<InfoDialogViewModel>();
			Bind<IMainViewModel>().To<MainViewModel>();
			Bind<IMessageDetailsViewModel>().To<MessageDetailsViewModel>();
			Bind<IMessenger>().ToConstant( Messenger.Default );
			Bind<IMuteSettings>().To<MuteSettings>();
			Bind<INotificationSettings>().To<NotificationSettings>();
			Bind<INotifier>().To<Notifier>();
			Bind<IProfileDialogViewModel>().To<ProfileDialogViewModel>();
			Bind<IRetweetDialogViewModel>().To<RetweetDialogViewModel>();
			Bind<ISearchDialogViewModel>().To<SearchDialogViewModel>();
			Bind<ISettingsDialogViewModel>().To<SettingsDialogViewModel>();
			Bind<ITextInputDialogViewModel>().To<TextInputDialogViewModel>();
			Bind<ITweetDetailsViewModel>().To<TweetDetailsViewModel>();
			Bind<ITwitterAuthorizer>().To<TwitterAuthorizer>();
			Bind<IVisualSettings>().To<VisualSettings>();
		}
	}
}