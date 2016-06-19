using System.Linq;
using LinqToTwitter;
using LinqToTwitter.Common;
using LitJson;

namespace Twice.Models.Twitter.Entities
{
	/// <summary>
	///     Extended user entity that includes some data that Linq2Twitter doesn't offer
	/// </summary>
	internal class UserEx : User, IHighlightable
	{
		public UserEx()
		{
			BioEntities = new LinqToTwitter.Entities();
			UrlEntities = new LinqToTwitter.Entities();
		}

		public UserEx( JsonData user )
			: base( user )
		{
			var ent = user.GetValue<JsonData>( "entities" );

			UrlEntities = new LinqToTwitter.Entities( ent.GetValue<JsonData>( "url" ) );
			BioEntities = new LinqToTwitter.Entities( ent.GetValue<JsonData>( "description" ) );

			UrlDisplay = Url;
			if( UrlEntities.UrlEntities.Any() )
			{
				UrlDisplay = UrlEntities.UrlEntities.First().DisplayUrl;
			}
		}

		public LinqToTwitter.Entities Entities => BioEntities;
		public string Text => Description;

		public LinqToTwitter.Entities BioEntities { get; }
		public string UrlDisplay { get; }
		public LinqToTwitter.Entities UrlEntities { get; }
	}
}