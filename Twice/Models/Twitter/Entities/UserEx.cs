using LinqToTwitter;
using LinqToTwitter.Common;
using LitJson;
using NLog;
using System.Linq;
using System.Windows.Input;
using Twice.ViewModels;

namespace Twice.Models.Twitter.Entities
{
	/// <summary>
	///  Extended user entity that includes some data that Linq2Twitter doesn't offer 
	/// </summary>
	internal class UserEx : User, IHighlightable
	{
		public UserEx()
		{
			BioEntities = new LinqToTwitter.Entities();
			UrlEntities = new LinqToTwitter.Entities();

			BlockUserCommand = new LogMessageCommand( "Tried to block user from UserEx", LogLevel.Warn );
			ReportSpamCommand = new LogMessageCommand( "Tried to report user from UserEx", LogLevel.Warn );
		}

		public UserEx( JsonData user )
			: base( user )
		{
			var ent = user.GetValue<JsonData>( "entities" );

			UrlEntities = new LinqToTwitter.Entities( ent.GetValue<JsonData>( "url" ) );
			BioEntities = new LinqToTwitter.Entities( ent.GetValue<JsonData>( "description" ) )
			{
				HashTagEntities = EntityParser.ExtractHashtags( Description ),
				UserMentionEntities = EntityParser.ExtractMentions( Description )
			};

			UrlDisplay = Url;
			if( UrlEntities.UrlEntities.Any() )
			{
				UrlDisplay = UrlEntities.UrlEntities.First().DisplayUrl;
			}
		}

		public LinqToTwitter.Entities BioEntities { get; }
		public ICommand BlockUserCommand { get; }
		public LinqToTwitter.Entities Entities => BioEntities;
		public ICommand ReportSpamCommand { get; }
		public string Text => Description;
		public string UrlDisplay { get; set; }
		public LinqToTwitter.Entities UrlEntities { get; }
	}
}