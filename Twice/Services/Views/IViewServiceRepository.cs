using System.Threading.Tasks;
using Twice.Views;

namespace Twice.Services.Views
{
	internal interface IViewServiceRepository
	{


		Task ShowSettings();

		Task<string> OpenFile( FileServiceArgs args = null );

		Task ViewProfile( ulong userId );

		Task<bool> Confirm( ConfirmServiceArgs args );

		Dialog CurrentDialog { get; }
	}
}