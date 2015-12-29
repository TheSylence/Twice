using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro.Controls;
using MahApps.Metro.SimpleChildWindow;
using Twice.ViewModels;

namespace Twice.Services.ViewServices
{
	internal abstract class ViewServiceBase<TDialog, TViewModel, TResult> : IViewService
		where TDialog : ChildWindow, new()
		where TViewModel : class, IDialogViewModel
		where TResult : class
	{
		public async Task<object> Show( object args = null )
		{
			var dlg = new TDialog();
			var vm = (TViewModel)dlg.DataContext;
			Debug.Assert( vm != null, "vm != null" );

			SetupViewModel( vm, args );

			TaskCompletionSource<TResult> tcs = new TaskCompletionSource<TResult>();
			vm.CloseRequested += ( s, e ) =>
			{
				dlg.Close();
				if( e.Result == true )
				{
					tcs.SetResult( SetupResult( vm ) );
				}
				else
				{
					tcs.SetCanceled();
				}
			};

			await Window.ShowChildWindowAsync( dlg );
			return await tcs.Task.ContinueWith( t => t.IsCanceled ? null : t.Result );
		}

		protected virtual TResult SetupResult( TViewModel vm )
		{
			return null;
		}

		protected virtual void SetupViewModel( TViewModel vm, object args )
		{
		}

		private static MetroWindow Window => Application.Current.MainWindow as MetroWindow;
	}
}