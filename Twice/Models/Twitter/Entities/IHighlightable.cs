using System.Windows.Input;

namespace Twice.Models.Twitter.Entities
{
	internal interface IHighlightable
	{
		ICommand BlockUserCommand { get; }
		LinqToTwitter.Entities Entities { get; }
		ICommand ReportSpamCommand { get; }
		string Text { get; }
	}
}