using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;
using Twice.Models.Twitter;
using Twice.Resources;
using Twice.ViewModels.Twitter;

namespace Twice.ViewModels.Columns
{
	internal class TimelineColumn : ColumnViewModelBase
	{
		public TimelineColumn( IContextEntry context )
			: base( context )
		{
			Statuses = new ObservableCollection<StatusViewModel>();
			Title = Strings.Home;
		}

		protected override async Task OnLoad()
		{
			var statues = await Context.Twitter.Status.Where( s => s.Type == StatusType.Home && s.UserID == Context.UserId ).ToListAsync();
			foreach( var status in statues.Select( s => new StatusViewModel( s, Context ) ) )
			{
				Statuses.Add( status );
			}
		}

		public override Icon Icon => Icon.Home;
		public override ICollection<StatusViewModel> Statuses { get; }
		public override string Title { get; protected set; }
	}
}