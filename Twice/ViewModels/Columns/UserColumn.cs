using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Threading;
using LinqToTwitter;
using Twice.Models.Twitter;
using Twice.ViewModels.Twitter;

namespace Twice.ViewModels.Columns
{
	internal class UserColumn : ColumnViewModelBase
	{
		public UserColumn( IContextEntry context, ulong userId )
			: base( context )
		{
			UserId = userId;
		}

		protected override async Task OnLoad()
		{
			var userInfo = await Context.Twitter.User.Where( u => u.UserID == UserId && u.Type == UserType.Show ).FirstAsync();
			Title = userInfo.ScreenNameResponse;

			var statuses = await Context.Twitter.Status.Where( s => s.Type == StatusType.User && s.UserID == UserId ).ToListAsync();
			var list = statuses.Where( s => !Muter.IsMuted( s ) ).Select( t => new StatusViewModel( t, Context ) ).ToArray();
			
			await AddStatuses( list );
		}

		public override Icon Icon => Icon.User;
		private readonly ulong UserId;
	}
}