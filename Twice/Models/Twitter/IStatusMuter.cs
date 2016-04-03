using LinqToTwitter;

namespace Twice.Models.Twitter
{
	interface IStatusMuter
	{
		bool IsMuted( Status status );
	}
}