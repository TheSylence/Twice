using System.Diagnostics;

namespace Twice.ViewModels.Twitter
{
	internal interface IMessageDetailsViewModel : IDialogViewModel
	{
		MessageViewModel Message { get; set; }
	}

	internal class MessageDetailsViewModel : DialogViewModel, IMessageDetailsViewModel
	{
		public MessageViewModel Message
		{
			[DebuggerStepThrough] get { return _Message; }
			set
			{
				if( _Message == value )
				{
					return;
				}

				_Message = value;
				RaisePropertyChanged();
			}
		}

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private MessageViewModel _Message;
	}
}