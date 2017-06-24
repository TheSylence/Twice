using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Ninject;
using Twice.Models.Configuration;
using Twice.Models.Twitter;
using Twice.Utilities.Os;
using Twice.Views.Services;

namespace Twice.ViewModels
{
	internal class ViewModelBaseEx : ViewModelBase, IViewModelBase
	{
		protected ViewModelBaseEx( IMessenger messenger = null )
			: base( messenger )
		{
		}

		[Inject]
		public ITwitterContextList ContextList { get; set; }

		[Inject]
		public IConfig Configuration { protected get; set; }

		[Inject]
		public IProcessStarter ProcessStarter { protected get; set; }

		[Inject]
		public ITwitterConfiguration TwitterConfig { protected get; set; }

		[Inject]
		public IViewServiceRepository ViewServiceRepository { protected get; set; }
	}
}