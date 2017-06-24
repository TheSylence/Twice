using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;

namespace Twice.Views
{
	[ExcludeFromCodeCoverage]
	public class Dialog : ContentControl
	{
		static Dialog()
		{
			DefaultStyleKeyProperty.OverrideMetadata( typeof( Dialog ), new FrameworkPropertyMetadata( typeof( Dialog ) ) );
		}

		public static readonly DependencyProperty TitleProperty = DependencyProperty.Register( "Title", typeof( string ),
			typeof( Dialog ), new PropertyMetadata( null ) );

		public string Title
		{
			get => (string)GetValue( TitleProperty );
			set => SetValue( TitleProperty, value );
		}
	}
}