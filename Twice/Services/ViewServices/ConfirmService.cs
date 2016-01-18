using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Twice.Resources;

namespace Twice.Services.ViewServices
{
	internal interface IConfirmService : IViewService
	{ }

	internal class ConfirmService : IConfirmService
	{
		public async Task<object> Show( object args = null )
		{
			ConfirmServiceArgs csa = args as ConfirmServiceArgs;
			Debug.Assert( csa != null );

			var settings = new MetroDialogSettings
			{
				AffirmativeButtonText = Strings.Yes,
				NegativeButtonText = Strings.No
			};
			var result = await Window.ShowMessageAsync( csa.Title, csa.Message, MessageDialogStyle.AffirmativeAndNegative, settings );

			return result == MessageDialogResult.Affirmative;
		}

		private static MetroWindow Window => Application.Current.MainWindow as MetroWindow;
	}

	internal class ConfirmServiceArgs
	{
		public ConfirmServiceArgs( string message, string title = null )
		{
			Message = message;
			Title = title ?? Strings.Confirm;
		}

		public string Message { get; }
		public string Title { get; }
	}
}