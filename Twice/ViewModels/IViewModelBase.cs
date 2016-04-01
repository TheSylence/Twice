using Twice.Models.Twitter;

namespace Twice.ViewModels
{
	internal interface IViewModelBase
	{
		ITwitterContextList ContextList { get; set; }
	}
}