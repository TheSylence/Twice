using GalaSoft.MvvmLight.Threading;
using LinqToTwitter;
using System.Linq;
using System.Threading.Tasks;
using Twice.Models.Twitter;
using Twice.Resources;
using Twice.ViewModels.Columns.Definitions;
using Twice.ViewModels.Twitter;

namespace Twice.ViewModels.Columns
{
	internal class TimelineColumn : ColumnViewModelBase
	{
		// TODO: Implement joined timelines for multiple contexts
		public TimelineColumn( IContextEntry context, ColumnDefinition definition )
			: base( context, definition )
		{
			Title = Strings.Home;
		}

		protected override async Task OnLoad()
		{
			var statues = await Context.Twitter.Status.Where( s => s.Type == StatusType.Home && s.UserID == Context.UserId ).ToListAsync();
			var list = statues.Where( s => !Muter.IsMuted( s ) ).Select( s => new StatusViewModel( s, Context ) ).ToArray();
			
			await AddStatuses( list );
		}

		public override Icon Icon => Icon.Home;
	}
}