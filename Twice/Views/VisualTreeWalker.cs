using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Media;

namespace Twice.Views
{
	[ExcludeFromCodeCoverage]
	internal static class VisualTreeWalker
	{
		public static TControl FindParent<TControl>( UIElement ctrl )
			where TControl : UIElement
		{
			while( ctrl != null )
			{
				if( ctrl is TControl )
				{
					return ctrl as TControl;
				}

				ctrl = VisualTreeHelper.GetParent( ctrl ) as UIElement;
			}

			return null;
		}
	}
}