using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;

namespace Twice.Services.ViewServices
{
	internal interface IFileSelectService : IViewService
	{
	}

	class FileServiceArgs
	{
		public FileServiceArgs( params string[] fileTypes )
		{
			Filter = string.Join( "|", fileTypes );
		}

		public string Filter { get; }
	}

	internal class FileSelectService : IFileSelectService
	{
		public Task<object> Show( object args = null )
		{
			OpenFileDialog dlg = new OpenFileDialog();
			var fsa = args as FileServiceArgs;
			if( fsa != null )
			{
				dlg.Filter = fsa.Filter;
			}

			if( dlg.ShowDialog( Application.Current.MainWindow ) == true )
			{
				return Task.FromResult<object>( dlg.FileName );
			}

			return Task.FromResult<object>( null );
		}
	}
}