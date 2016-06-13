using System.Collections.Generic;
using System.Threading.Tasks;
using LinqToTwitter;

namespace Twice.Models.Twitter.Repositories
{
	internal interface ITwitterSearchRepository
	{
		Task<List<Status>> SearchReplies( Status status );

		Task<List<Status>> SearchStatuses( string query );

		Task<List<User>> SearchUsers( string query );
	}
}