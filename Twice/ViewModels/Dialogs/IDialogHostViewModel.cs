using System.Threading.Tasks;
using System.Windows.Input;

namespace Twice.ViewModels.Dialogs
{
	internal interface IDialogHostViewModel
	{
		Task Setup<TViewModel>( TViewModel vm ) where TViewModel : class;

		ICommand BackCommand { get; }
	}
}