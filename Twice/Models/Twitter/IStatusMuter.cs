using LinqToTwitter;

namespace Twice.Models.Twitter
{
	internal interface IStatusMuter
	{
		bool IsMuted( Status status );
	}
}