using System.Threading.Tasks;
using Ninject;
using Twice.Services.ViewServices;

namespace Twice.Services
{
	internal class ServiceRepository : IServiceRepository
	{
		public ServiceRepository()
		{
			Kernel = App.Kernel;
		}

		public async Task<TResult> Show<TService, TResult>( object args = null )
			where TService : IViewService
			where TResult : class
		{
			var service = Kernel.Get<TService>();
			var result = await service.Show();

			return result as TResult;
		}

		private readonly IKernel Kernel;
	}
}