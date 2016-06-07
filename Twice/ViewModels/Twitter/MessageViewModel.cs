﻿using LinqToTwitter;
using System;
using Twice.Models.Twitter;
using Twice.Services.Views;

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

		public override DateTime CreatedAt => Model.CreatedAt;
		public override Entities Entities => Model.Entities;
		public override ulong Id => Model.GetMessageId();
		public bool IsIncoming { get; }
		public DirectMessage Model { get; }
		public UserViewModel Partner { get; }
		public override string Text => Model.Text;
		private readonly IContextEntry Context;
		private readonly IViewServiceRepository ViewServiceRepository;
	}
}