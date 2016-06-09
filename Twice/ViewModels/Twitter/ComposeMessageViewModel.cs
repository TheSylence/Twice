using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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
		}

		protected override async Task<bool> OnOk()
		{
			IsSending = true;
			var msg = await Context.Twitter.SendMessage( Recipient, Text );
			MessengerInstance.Send( new DmMessage( msg, EntityAction.Create ) );
			IsSending = false;

			return await base.OnOk();
		}

		private void CheckRecipient()
		{
			CanSend = null;
			IsCheckingRelationship = true;
			Task.Run( async () =>
			{
				var friendship = await Context.Twitter.Friendships.GetFriendshipWith( Context.UserId, Recipient );
				CanSend = friendship?.TargetRelationship?.Following == true;

				IsCheckingRelationship = false;
			} );
		}

		public Task OnLoad( object data )
		{
			return Task.CompletedTask;
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

		private IContextEntry Context => ContextList.Contexts.First();

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

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private bool? _CanSend;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private bool _IsCheckingRelationship;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private bool _IsSending;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private string _Recipient;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private string _Text;
	}
}