using System.Threading.Tasks;

namespace Twice.ViewModels.Main
{
	internal interface ILoadCallback
	{
		Task OnLoad( object data );
	}
}