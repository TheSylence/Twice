using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using LinqToTwitter;
using Twice.Models.Contexts;
using Twice.ViewModels.Twitter;

namespace Twice.ViewModels.Columns
{
	internal class UserColumn : IColumnViewModel
	{
		public UserColumn( IContextEntry context )
		{
			Title = context.AccountName;

			var statuses = context.Context.Status.Where( s => s.Type == StatusType.User && s.UserID == context.UserId ).ToArray();

			Statuses = new ObservableCollection<StatusViewModel>( statuses.Select( t => new StatusViewModel( t ) ) );
		}

		public ICollection<StatusViewModel> Statuses { get; }
		public string Title { get; }
	}
}