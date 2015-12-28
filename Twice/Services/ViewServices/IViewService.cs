using System.Threading.Tasks;

namespace Twice.Services.ViewServices
{
	interface IViewService
	{
		Task<object> Show( object args = null );
	}
}