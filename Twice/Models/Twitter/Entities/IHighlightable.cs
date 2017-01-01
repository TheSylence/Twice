using System.Threading.Tasks;
using System.Windows.Input;

namespace Twice.Models.Twitter.Entities
{
	internal interface IHighlightable
	{
		LinqToTwitter.Entities Entities { get; }
		string Text { get; }

		ICommand BlockUserCommand { get; }
		ICommand ReportSpamCommand { get; }
	}
}