using System.Diagnostics;
using System.Threading.Tasks;
using Twice.ViewModels.Main;

namespace Twice.ViewModels.Twitter
{
	internal interface IMessageDetailsViewModel : IDialogViewModel, ILoadCallback
	{
		MessageViewModel Message { get; set; }
	}

	internal class MessageDetailsViewModel : DialogViewModel, IMessageDetailsViewModel
	{
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

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private MessageViewModel _Message;
	}
}