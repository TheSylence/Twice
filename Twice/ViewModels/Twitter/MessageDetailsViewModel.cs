using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Twice.ViewModels.Twitter
{
	internal class MessageDetailsViewModel : DialogViewModel, IMessageDetailsViewModel
	{
		public MessageDetailsViewModel()
		{
			PreviousMessages = new ObservableCollection<MessageViewModel>();
		}

		public Task OnLoad( object data )
		{
			return Task.CompletedTask;
		}

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

		public ICollection<MessageViewModel> PreviousMessages { get; }

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private MessageViewModel _Message;
	}
}