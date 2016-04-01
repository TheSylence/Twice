using Twice.ViewModels.Columns.Definitions;
using Twice.ViewModels.Twitter;

namespace Twice.ViewModels
{
	internal interface INotifier
	{
		void DisplayMessage( string message, NotificationType type );

		void OnStatus( StatusViewModel status, ColumnNotifications columnSettings );
	}
}