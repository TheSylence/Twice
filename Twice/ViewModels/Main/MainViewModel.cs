using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Twice.Models.Contexts;
using Twice.ViewModels.Columns;

namespace Twice.ViewModels.Main
{
	internal class MainViewModel : ViewModelBaseEx, IMainViewModel
	{
		public MainViewModel( ITwitterContextList list )
		{
			Columns = new ObservableCollection<IColumnViewModel>();

			Columns.Add( new UserColumn( list.Contexts.First() ) );
			Columns.Add( new TestColumn() );
			Columns.Add( new TestColumn() );
		}

		public ICollection<IColumnViewModel> Columns { get; }
	}
}