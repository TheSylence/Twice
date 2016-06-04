using GalaSoft.MvvmLight;
using System.Diagnostics;
using Twice.ViewModels.Twitter;

namespace Twice.ViewModels.Flyouts
{
	internal class NotificationViewModel : ObservableObject
	{
		public NotificationViewModel( StatusViewModel status )
		{
			Type = NotificationType.Information;
			SetText( status.Model.Text );
		}

		public NotificationViewModel( string message, NotificationType type )
		{
			SetText( message );
			Type = type;
		}

		private void SetText( string text )
		{
			Text = text;
		}

		public string Text
		{
			[DebuggerStepThrough] get { return _Text; }
			set
			{
				if( _Text == value )
				{
					return;
				}

				_Text = value;
				RaisePropertyChanged();
			}
		}

		public NotificationType Type
		{
			[DebuggerStepThrough] get { return _Type; }
			set
			{
				if( _Type == value )
				{
					return;
				}

				_Type = value;
				RaisePropertyChanged();
			}
		}

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private string _Text;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private NotificationType _Type;
	}
}