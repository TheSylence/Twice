using System;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Ninject;
using Twice.Messages;
using Twice.Models.Twitter;
using Twice.Resources;
using Twice.ViewModels.Validation;

namespace Twice.ViewModels.Twitter
{
	internal class ComposeMessageViewModel : DialogViewModel, IComposeMessageViewModel
	{
		public ComposeMessageViewModel()
		{
			Title = Strings.ComposeMessage;

			Validate( () => Recipient ).NotEmpty();
			Validate( () => Text ).NotEmpty();

			// Don't validate based on CanSend property because twitter states: "Besides determining
			// > the follow status between two users via friendships/ lookup, you are unable to >
			// determine if you can Direct Message a user via the public API" So we simply use this
			// (https://dev.twitter.com/rest/reference/post/direct_messages/new) value as an
			// indicator for the user and handle any errors that twitter will report
		}

		protected override async Task<bool> OnOk()
		{
			IsSending = true;
			try
			{
				var msg = await Context.Twitter.Messages.SendMessage( Recipient, Text );
				MessengerInstance.Send( new DmMessage( msg, EntityAction.Create ) );
			}
			catch( Exception ex )
			{
				Notifier.DisplayMessage( ex.GetReason(), NotificationType.Error );
			}
			finally
			{
				IsSending = false;
			}

			return await base.OnOk();
		}

		private void CheckRecipient()
		{
			CanSend = null;
			IsCheckingRelationship = true;
			Task.Run( async () =>
			{
				try
				{
					var friendship = await Context.Twitter.Friendships.GetFriendshipWith( Context.UserId, Recipient );
					CanSend = friendship?.TargetRelationship?.Following == true;
				}
				catch( Exception ex )
				{
					Notifier.DisplayMessage( ex.GetReason(), NotificationType.Error );
					CanSend = false;
				}

				IsCheckingRelationship = false;
			} );
		}

		[UsedImplicitly]
		private void OnRecipientChanged()
		{
			CheckRecipient();
		}

		public bool? CanSend { get; set; }

		public MessageViewModel InReplyTo { get; set; }

		public bool IsCheckingRelationship { get; set; }

		public bool IsSending { get; set; }

		public string Recipient { get; set; }

		public string Text { get; set; }

		public Task OnLoad( object data )
		{
			return Task.CompletedTask;
		}

		private IContextEntry Context => ContextList.Contexts.First();

		[Inject]

		// ReSharper disable once MemberCanBePrivate.Global
		public INotifier Notifier { get; set; }
	}
}