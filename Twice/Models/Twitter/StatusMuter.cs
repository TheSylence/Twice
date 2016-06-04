using Anotar.NLog;
using LinqToTwitter;
using System;
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

		private static bool CheckMute( MuteEntry entry, Status status )
		{
			char[] typeIndicators = {'#', ':', '@'};
			string value = entry.Filter;

			char typeIndicator = value[0];
			if( typeIndicators.Contains( typeIndicator ) )
			{
				value = value.Substring( 1 );
			}

			StringComparer comp = entry.CaseSensitive
				? StringComparer.Ordinal
				: StringComparer.OrdinalIgnoreCase;

			switch( typeIndicator )
			{
			case '#':
				return status.Entities.HashTagEntities.Any( h => comp.Compare( h.Tag, value ) == 0 );

			case ':':
				return comp.Compare( new TweetSource( status.Source ).Name, value ) == 0;

			case '@':
				return status.Entities.UserMentionEntities.Any( m => comp.Compare( m.ScreenName, value ) == 0 );

			default:
				if( !entry.CaseSensitive )
				{
					return status.Text.ToLower().Contains( value.ToLower() );
				}

				return status.Text.Contains( value );
			}
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
				LogTo.Debug( $"Muted status {status.GetStatusId()}" );
			}

			return result;
		}

		private readonly MuteConfig Muting;
	}
}