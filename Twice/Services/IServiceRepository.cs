using System.Threading.Tasks;
using Twice.Services.ViewServices;

namespace Twice.Services
{
	interface IServiceRepository
	{
		Task<TResult> Show<TService, TResult>( object args = null ) 
			where TService : IViewService
			where TResult : class;
	}
}