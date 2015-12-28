using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using LinqToTwitter;
using Twice.Models.Twitter;
using Twice.ViewModels.Twitter;

namespace Twice.ViewModels.Columns
{
	internal class UserColumn : ColumnViewModelBase
	{
		public UserColumn( IContextEntry context )
		{
			Title = context.AccountName;

			var statuses = context.Twitter.Status.Where( s => s.Type == StatusType.User && s.UserID == context.UserId ).ToArray();

			Statuses = new ObservableCollection<StatusViewModel>( statuses.Select( t => new StatusViewModel( t, context ) ) );
		}

		public override Icon Icon => Icon.User;

		public override ICollection<StatusViewModel> Statuses { get; }
		public override string Title { get; }
	}
}