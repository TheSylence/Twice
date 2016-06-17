using Twice.Models.Columns;
using Twice.ViewModels.Twitter;

namespace Twice.ViewModels
{
	internal interface INotifier
	{
		void DisplayMessage( string message, NotificationType type, bool? positionOverwriteTop = null );

		void DisplayWin10Message( string message );

		void OnItem( ColumnItem item, ColumnNotifications columnSettings );
	}
}