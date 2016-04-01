using GalaSoft.MvvmLight;
using Ninject;
using Twice.Models.Configuration;
using Twice.Models.Twitter;
using Twice.Services.Views;

namespace Twice.ViewModels
{
	internal interface IViewModelBase
	{
		ITwitterContextList ContextList { get; set; }
	}

	internal class ViewModelBaseEx : ViewModelBase, IViewModelBase
	{
		[Inject]
		public IConfig Configuration { get; set; }

		[Inject]
		public ITwitterContextList ContextList { get; set; }

		[Inject]
		public IViewServiceRepository ViewServiceRepository { get; set; }
	}
}