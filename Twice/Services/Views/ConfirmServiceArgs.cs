using Twice.Resources;

namespace Twice.Services.Views
{
	internal class ConfirmServiceArgs
	{
		public ConfirmServiceArgs( string message, string title = null )
		{
			Message = message;
			Title = title ?? Strings.Confirm;
		}

		public string Message { get; }
		public string Title { get; }
	}
}