using System.Diagnostics;
using GalaSoft.MvvmLight;
using MahApps.Metro.Controls;
using Twice.ViewModels.Twitter;

namespace Twice.ViewModels.Flyouts
{
	internal class NotificationViewModel : ObservableObject
	{
		public NotificationViewModel( ColumnItem item, bool top )
		{
			Type = NotificationType.Information;

			var status = item as StatusViewModel;
			if( status != null )
			{
				SetText( status.Model.Text );
			}

			var message = item as MessageViewModel;
			if( message != null )
			{
				SetText( message.Model.Text );
			}

			FlyoutPosition = top
				? Position.Top
				: Position.Bottom;
		}

		public NotificationViewModel( string message, NotificationType type, bool top )
		{
			SetText( message );
			Type = type;

			FlyoutPosition = top
				? Position.Top
				: Position.Bottom;
		}

		private void SetText( string text )
		{
			Text = text;
		}

		public Position FlyoutPosition
		{
			[DebuggerStepThrough] get { return _FlyoutPosition; }
			set
			{
				if( _FlyoutPosition == value )
				{
					return;
				}

				_FlyoutPosition = value;
				RaisePropertyChanged();
			}
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
		private Position _FlyoutPosition;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private string _Text;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private NotificationType _Type;
	}
}