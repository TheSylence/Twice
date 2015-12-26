using LinqToTwitter;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Twice.Models.Contexts;
using Twice.ViewModels.Twitter;

namespace Twice.ViewModels.Columns
{
	internal class UserColumn : IColumnViewModel
	{
		public UserColumn( IContextEntry context )
		{
			Title = context.AccountName;

			var statuses = context.Twitter.Status.Where( s => s.Type == StatusType.User && s.UserID == context.UserId ).ToArray();

			Statuses = new ObservableCollection<StatusViewModel>( statuses.Select( t => new StatusViewModel( t, context ) ) );
		}

		public Icon Icon => Icon.User;

		public ICollection<StatusViewModel> Statuses { get; }
		public string Title { get; }
	}
}