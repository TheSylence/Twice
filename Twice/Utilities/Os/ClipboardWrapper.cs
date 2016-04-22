using System.Diagnostics.CodeAnalysis;
using System.Windows;

namespace Twice.Utilities.Os
{
	[ExcludeFromCodeCoverage]
	internal class ClipboardWrapper : IClipboard
	{
		public void SetText( string text )
		{
			Clipboard.SetText( text );
		}
	}
}