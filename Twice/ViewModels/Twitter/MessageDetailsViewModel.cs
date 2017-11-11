using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Fody;
using LinqToTwitter;
using Newtonsoft.Json;
using Twice.Models.Twitter;
using Twice.Resources;

namespace Twice.ViewModels.Twitter
{
	internal class MessageDetailsViewModel : DialogViewModel, IMessageDetailsViewModel
	{
		public MessageDetailsViewModel()
		{
			PreviousMessages = new ObservableCollection<MessageViewModel>();
			Title = Strings.Message;
		}

		[ConfigureAwait( false )]
		public async Task OnLoad( object data )
		{
			IsLoadingPrevious = true;
			Message.WasRead = true;
			var userId = Message.Partner.Model.GetUserId();

			var cacheMessages = await Cache.GetMessages();
			var messages = cacheMessages.Where( m => m.Recipient == userId || m.Sender == userId )
				.Select( m => JsonConvert.DeserializeObject<DirectMessage>( m.Data ) )
				.Where( m => m.GetMessageId() < Message.Id )
				.OrderBy( m => m.CreatedAt );

			foreach( var m in messages )
			{
				var msg = new MessageViewModel( m, Message.Context, Configuration, ViewServiceRepository )
				{
					WasRead = true
				};
				await Dispatcher.RunAsync( () => PreviousMessages.Add( msg ) );
			}

			RaisePropertyChanged( nameof(PreviousMessages) );
			IsLoadingPrevious = false;
			await Dispatcher.RunAsync( () => ScrollRequested?.Invoke( this, EventArgs.Empty ) );
		}

		public bool IsLoadingPrevious { get; set; }
		public MessageViewModel Message { get; set; }

		public ICollection<MessageViewModel> PreviousMessages { get; }

		public event EventHandler ScrollRequested;
	}
}