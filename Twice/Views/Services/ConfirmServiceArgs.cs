using System.Diagnostics.CodeAnalysis;
using Twice.Resources;

namespace Twice.Views.Services
{
	[ExcludeFromCodeCoverage]
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