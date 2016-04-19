using Anotar.NLog;
using LinqToTwitter;
using System.Linq;
using Twice.Models.Configuration;

namespace Twice.Models.Twitter
{
	internal class StatusMuter : IStatusMuter
	{
		public StatusMuter( IConfig config )
		{
			Muting = config.Mute;
		}

		public bool IsMuted( Status status )
		{
			if( status == null )
			{
				return true;
			}

			bool result = Muting.Entries.Any( mute => CheckMute( mute, status ) );
			if( result )
			{
				LogTo.Debug( $"Muted status {status.ID}" );
			}

			return result;
		}

		private static bool CheckMute( MuteEntry entry, Status status )
		{
			char[] typeIndicators = { '#', ':', '@' };
			string value = entry.Filter;

			char typeIndicator = value[0];
			if( typeIndicators.Contains( typeIndicator ) )
			{
				value = value.Substring( 1 );
			}

			switch( typeIndicator )
			{
			case '#':
				return status.Entities.HashTagEntities.Any( h => h.Tag == value );

			case ':':
				return new TweetSource( status.Source ).Name == value;

			case '@':
				return status.Entities.UserMentionEntities.Any( m => m.ScreenName == value );

			default:
				return status.Text.Contains( value );
			}
		}

		private readonly MuteConfig Muting;
	}
}