using System.Windows;
using System.Windows.Controls;

namespace Twice.Views
{
	public class Dialog : ContentControl
	{
		static Dialog()
		{
			DefaultStyleKeyProperty.OverrideMetadata( typeof( Dialog ), new FrameworkPropertyMetadata( typeof( Dialog ) ) );
		}

		public string Title
		{
			get { return (string)GetValue( TitleProperty ); }
			set { SetValue( TitleProperty, value ); }
		}

		public static readonly DependencyProperty TitleProperty = DependencyProperty.Register( "Title", typeof( string ), typeof( Dialog ), new PropertyMetadata( null ) );
	}
}