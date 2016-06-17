using System;
using System.Diagnostics;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using LinqToTwitter;
using Twice.Models.Twitter;
using Twice.Views.Services;

namespace Twice.ViewModels.Twitter
{
	internal class MessageViewModel : ColumnItem
	{
		public MessageViewModel( DirectMessage model, IContextEntry context, IViewServiceRepository viewServices )
		{
			Model = model;
			Context = context;
			ViewServiceRepository = viewServices;

			User = new UserViewModel( model.Sender );
			Partner = new UserViewModel( model.SenderID == context.UserId
				? model.Recipient
				: model.Sender );

			IsIncoming = Model.SenderID != context.UserId;
		}

		private async void ExecuteReplyCommand()
		{
			await ViewServiceRepository.ReplyToMessage( this );
		}

		public IContextEntry Context { get; private set; }
		public override DateTime CreatedAt => Model.CreatedAt;

		public override Entities Entities => Model.Entities;

		public override ulong Id => Model.GetMessageId();

		public bool IsIncoming { get; }

		public DirectMessage Model { get; }

		public UserViewModel Partner { get; }

		public ICommand ReplyCommand => _ReplyCommand ?? ( _ReplyCommand = new RelayCommand( ExecuteReplyCommand ) );

		public override string Text => Model.Text;

		public bool WasRead
		{
			[DebuggerStepThrough] get { return _WasRead; }
			set
			{
				if( _WasRead == value )
				{
					return;
				}

				_WasRead = value;
				RaisePropertyChanged();
			}
		}

		private readonly IViewServiceRepository ViewServiceRepository;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private RelayCommand _ReplyCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private bool _WasRead;
	}
}