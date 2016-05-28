using Ninject;
using System.Diagnostics.CodeAnalysis;
using Twice.ViewModels.Accounts;
using Twice.ViewModels.ColumnManagement;
using Twice.ViewModels.Dialogs;
using Twice.ViewModels.Info;
using Twice.ViewModels.Profile;
using Twice.ViewModels.Settings;
using Twice.ViewModels.Twitter;

namespace Twice.ViewModels
{
	[ExcludeFromCodeCoverage]
	internal class DialogViewModelLocator
	{
		public DialogViewModelLocator()
		{
			Kernel = App.Kernel;
		}

		public IColumnTypeSelectionDialogViewModel AccountColumns => Kernel.Get<IColumnTypeSelectionDialogViewModel>();
		public IAccountsDialogViewModel Accounts => Kernel.Get<IAccountsDialogViewModel>();
		public IAddColumnDialogViewModel AddColumn => Kernel.Get<IAddColumnDialogViewModel>();
		public IImageDialogViewModel Image => Kernel.Get<IImageDialogViewModel>();
		public IInfoDialogViewModel Info => Kernel.Get<IInfoDialogViewModel>();
		public IProfileDialogViewModel Profile => Kernel.Get<IProfileDialogViewModel>();
		public ISettingsDialogViewModel Settings => Kernel.Get<ISettingsDialogViewModel>();
		public ITextInputDialogViewModel TextInput => Kernel.Get<ITextInputDialogViewModel>();
		public ITweetDetailsViewModel TweetDetails => Kernel.Get<ITweetDetailsViewModel>();
		private readonly IKernel Kernel;
	}
}