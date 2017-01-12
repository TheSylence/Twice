using GalaSoft.MvvmLight.CommandWpf;
using LinqToTwitter;
using NLog;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Twice.Models.Configuration;
using Twice.Models.Twitter;
using Twice.Models.Twitter.Comparers;
using Twice.Views.Services;

namespace Twice.ViewModels.Twitter
{
	internal class MessageViewModel : ColumnItem
	{
		public MessageViewModel( DirectMessage model, IContextEntry context, IConfig config, IViewServiceRepository viewServices )
			: base( config, viewServices )
		{
			Model = model;
			Context = context;

			User = new UserViewModel( model.Sender );
			Partner = new UserViewModel( model.SenderID == context.UserId
				? model.Recipient
				: model.Sender );

			IsIncoming = Model.SenderID != context.UserId;

			BlockUserCommand = new LogMessageCommand( "Tried to block user from MessageViewModel", LogLevel.Warn );
			ReportSpamCommand = new LogMessageCommand( "Tried to report user from MessageViewModel", LogLevel.Warn );
		}

		public override async Task LoadDataAsync()
		{
			await LoadInlineMedias();
		}

		protected override async Task LoadInlineMedias()
		{
			if( Config?.Visual?.InlineMedia != true )
			{
				return;
			}

			var mediaEntities = Model?.Entities?.MediaEntities ?? Enumerable.Empty<MediaEntity>();
			var entities = mediaEntities.Distinct( TwitterComparers.MediaEntityComparer );

			foreach( var vm in entities.Select( entity => new StatusMediaViewModel( entity, Context.UserId ) ) )
			{
				vm.OpenRequested += Image_OpenRequested;
				_InlineMedias.Add( vm );
			}

			var urlEntities = Model?.Entities?.UrlEntities ?? Enumerable.Empty<UrlEntity>();
			var urls = urlEntities.Distinct( TwitterComparers.UrlEntityComparer ).Select( e => e.ExpandedUrl );

			foreach( var url in urls )
			{
				var extracted = await MediaExtractor.ExtractMedia( url );
				if( extracted != null )
				{
					var vm = new StatusMediaViewModel( extracted );
					vm.OpenRequested += Image_OpenRequested;
					_InlineMedias.Add( vm );
				}
			}

			RaisePropertyChanged( nameof( InlineMedias ) );
		}

		private async void ExecuteReplyCommand()
		{
			await ViewServiceRepository.ReplyToMessage( this );
		}

		public override ICommand BlockUserCommand { get; }
		public IContextEntry Context { get; }
		public override DateTime CreatedAt => Model.CreatedAt;
		public override Entities Entities => Model.Entities;
		public override ulong Id => Model.GetMessageId();
		public bool IsIncoming { get; }
		public DirectMessage Model { get; }
		public override ulong OrderId => Id;
		public UserViewModel Partner { get; }
		public ICommand ReplyCommand => _ReplyCommand ?? ( _ReplyCommand = new RelayCommand( ExecuteReplyCommand ) );
		public override ICommand ReportSpamCommand { get; }
		public override string Text => Model.Text;

		public bool WasRead
		{
			[DebuggerStepThrough]
			get { return _WasRead; }
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

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private RelayCommand _ReplyCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private bool _WasRead;
	}
}