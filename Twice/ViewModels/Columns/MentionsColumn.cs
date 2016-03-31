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
	internal class MentionsColumn : ColumnViewModelBase
	{
		public MentionsColumn( IContextEntry context, ColumnDefinition definition ) : base( context, definition )
		{
			Icon = Icon.Mentions;
			Title = Strings.Mentions;
		}

		protected override async Task OnLoad()
		{
			var statuses = await Context.Twitter.Status.Where( s => s.Type == StatusType.Mentions && s.UserID == Context.UserId ).ToListAsync();
			var list = statuses.Where( s => !Muter.IsMuted( s ) ).Select( s => new StatusViewModel( s, Context ) );

			await AddStatuses( list );
		}

		public override Icon Icon { get; }
	}
}