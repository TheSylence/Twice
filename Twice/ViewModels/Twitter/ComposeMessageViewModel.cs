using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Ninject;
using Twice.Messages;
using Twice.Models.Twitter;
using Twice.ViewModels.Validation;

namespace Twice.ViewModels.Twitter
{
	internal class ComposeMessageViewModel : DialogViewModel, IComposeMessageViewModel
	{
		public ComposeMessageViewModel()
		{
			Validate( () => Recipient ).NotEmpty();
			Validate( () => Text ).NotEmpty();

			// Don't validate based on CanSend property because twitter states: "Besides determining
			// > the follow status between two users via friendships/ lookup, you are unable to >
			// determine if you can Direct Message a user via the public API" So we simply use this
			// (https://dev.twitter.com/rest/reference/post/direct_messages/new) value as an
			// indicator for the user and handle any errors that twitter will report
		}

		public Task OnLoad( object data )
		{
			return Task.CompletedTask;
		}

		protected override async Task<bool> OnOk()
		{
			IsSending = true;
			try
			{
				var msg = await Context.Twitter.SendMessage( Recipient, Text );
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

		public bool? CanSend
		{
			[DebuggerStepThrough] get { return _CanSend; }
			set
			{
				if( _CanSend == value )
				{
					return;
				}

				_CanSend = value;
				RaisePropertyChanged();
			}
		}

		private IContextEntry Context => ContextList.Contexts.First();

		public MessageViewModel InReplyTo
		{
			[DebuggerStepThrough] get { return _InReplyTo; }
			set
			{
				if( _InReplyTo == value )
				{
					return;
				}

				_InReplyTo = value;
				RaisePropertyChanged();
			}
		}

		public bool IsCheckingRelationship
		{
			[DebuggerStepThrough] get { return _IsCheckingRelationship; }
			set
			{
				if( _IsCheckingRelationship == value )
				{
					return;
				}

				_IsCheckingRelationship = value;
				RaisePropertyChanged();
			}
		}

		public bool IsSending
		{
			[DebuggerStepThrough] get { return _IsSending; }
			set
			{
				if( _IsSending == value )
				{
					return;
				}

				_IsSending = value;
				RaisePropertyChanged();
			}
		}

		[Inject]
		public INotifier Notifier { get; set; }

		public string Recipient
		{
			[DebuggerStepThrough] get { return _Recipient; }
			set
			{
				if( _Recipient == value )
				{
					return;
				}

				_Recipient = value;
				RaisePropertyChanged();
				CheckRecipient();
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

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private bool? _CanSend;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private MessageViewModel _InReplyTo;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private bool _IsCheckingRelationship;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private bool _IsSending;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private string _Recipient;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private string _Text;
	}
}