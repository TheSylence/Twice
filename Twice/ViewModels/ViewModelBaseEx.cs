using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Ninject;
using Twice.Models.Configuration;
using Twice.Models.Twitter;
using Twice.Services.Views;
using Twice.Utilities.Os;

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
		public IConfig Configuration { get; set; }

		[Inject]
		public IProcessStarter ProcessStarter { get; set; }

		[Inject]
		public IViewServiceRepository ViewServiceRepository { get; set; }
	}
}