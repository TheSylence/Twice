using System.Windows;
using System.Windows.Controls;

namespace Twice.Views
{
	public class StatusContainer : ContentControl
	{
		public static readonly DependencyProperty ShowRetweetsProperty =
			DependencyProperty.Register( "ShowRetweets", typeof(bool), typeof(StatusContainer), new PropertyMetadata( false ) );

		public bool ShowRetweets
		{
			get { return (bool)GetValue( ShowRetweetsProperty ); }
			set { SetValue( ShowRetweetsProperty, value ); }
		}
	}
}