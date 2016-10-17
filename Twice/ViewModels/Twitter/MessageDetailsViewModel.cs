using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Fody;
using LinqToTwitter;
using Newtonsoft.Json;
using Twice.Models.Twitter;

namespace Twice.ViewModels.Twitter
{
	internal class MessageDetailsViewModel : DialogViewModel, IMessageDetailsViewModel
	{
		public MessageDetailsViewModel()
		{
			PreviousMessages = new ObservableCollection<MessageViewModel>();
		}

		public event EventHandler ScrollRequested;

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

			RaisePropertyChanged( nameof( PreviousMessages ) );
			IsLoadingPrevious = false;
			await Dispatcher.RunAsync( () => ScrollRequested?.Invoke( this, EventArgs.Empty ) );
		}

		public bool IsLoadingPrevious
		{
			[DebuggerStepThrough] get { return _IsLoadingPrevious; }
			private set
			{
				if( _IsLoadingPrevious == value )
				{
					return;
				}

				_IsLoadingPrevious = value;
				RaisePropertyChanged();
			}
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
		private bool _IsLoadingPrevious;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private MessageViewModel _Message;
	}
}