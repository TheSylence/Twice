using Twice.Models.Columns;
using Twice.Models.Configuration;
using Twice.ViewModels.Twitter;

namespace Twice.ViewModels
{
	internal interface INotifier
	{
		void DisplayMessage( string message, NotificationType type );

		void DisplayWin10Message( string message );

		void OnItem( ColumnItem item, ColumnNotifications columnSettings );

		void PreviewInAppNotification( string message, bool top, int closeTime );

		void PreviewPopupNotification( string message, int closeTime, bool win10, string display, Corner displayCorner );
	}
}