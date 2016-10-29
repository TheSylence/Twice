using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;

namespace Twice.Behaviors
{
	/// <summary>
	///     Behavior that can be attached to a TextBox to allow autocomplete functionality.
	/// </summary>
	[ExcludeFromCodeCoverage]
	internal class AutoComplete : BehaviorBase<TextBox>
	{
		public AutoComplete()
		{
			AutoCompleteBox = new ListView();
			AutoCompleteBox.MouseDoubleClick += AutoCompleteBox_MouseDoubleClick;

			AutoCompletePopup = new Popup
			{
				Child = AutoCompleteBox,
				StaysOpen = false,
				Placement = PlacementMode.Bottom,
				MaxHeight = 300
			};

			ShadowAssist.SetShadowDepth( AutoCompletePopup, ShadowDepth.Depth3 );

			AutoCompletePopup.Closed += AutoCompletePopup_Closed;
		}

		private enum SourceMode
		{
			None,
			Hashtags,
			Users
		}

		protected override void OnAttached()
		{
			AssociatedObject.PreviewTextInput += AssociatedObject_TextInput;
			AssociatedObject.PreviewKeyDown += AssociatedObject_PreviewKeyDown;

			var binding = new Binding( "ActualWidth" )
			{
				Source = AssociatedObject
			};
			AutoCompletePopup.SetBinding( FrameworkElement.WidthProperty, binding );

			AutoCompletePopup.PlacementTarget = AssociatedObject;
		}

		private void AssociatedObject_PreviewKeyDown( object sender, KeyEventArgs e )
		{
			if( !AutoCompletePopup.IsOpen )
			{
				return;
			}

			bool close = false;

			switch( e.Key )
			{
			case Key.Escape:
				close = true;
				break;

			case Key.Return:
				InsertText();

				close = true;
				break;

			case Key.Up:
				AutoCompleteBox.SelectedIndex--;
				e.Handled = true;
				break;

			case Key.Down:
				AutoCompleteBox.SelectedIndex++;
				e.Handled = true;
				break;

			case Key.Back:
				if( FilterText.Length > 0 )
				{
					FilterText = FilterText.Substring( 0, FilterText.Length - 1 );
				}
				else
				{
					close = true;
				}
				break;
			}

			int itemCount = FilteredItems.Count();
			if( AutoCompleteBox.SelectedIndex < 0 )
			{
				AutoCompleteBox.SelectedIndex = itemCount - 1;
			}
			if( AutoCompleteBox.SelectedIndex >= itemCount )
			{
				AutoCompleteBox.SelectedIndex = 0;
			}

			if( close )
			{
				e.Handled = true;
				CloseAutoCompleteBox();
			}
		}

		private void CloseAutoCompleteBox()
		{
			FilterText = string.Empty;
			AutoCompletePopup.IsOpen = false;
			Mode = SourceMode.None;
			AssociatedObject.Focus();
		}

		private void AssociatedObject_TextInput( object sender, TextCompositionEventArgs e )
		{
			var text = e.Text;
			if( text.StartsWith( Constants.Twitter.HashTag, StringComparison.Ordinal ) )
			{
				FilterText = string.Empty;
				AutoCompletePopup.IsOpen = true;
				AutoCompleteBox.ItemsSource = FilteredHashtags;
				AutoCompleteBox.SelectedIndex = 0;
				Mode = SourceMode.Hashtags;
			}
			else if( text.StartsWith( Constants.Twitter.Mention, StringComparison.Ordinal ) )
			{
				FilterText = string.Empty;
				AutoCompletePopup.IsOpen = true;
				AutoCompleteBox.ItemsSource = FilteredUsers;
				AutoCompleteBox.SelectedIndex = 0;
				Mode = SourceMode.Users;
			}
			else if( AutoCompletePopup.IsOpen )
			{
				string selectedText = (string)AutoCompleteBox.SelectedItem;

				FilterText += text;

				var items = FilteredItems.ToList();

				Debug.WriteLine( string.Join( " - ", items ) );

				AutoCompleteBox.ItemsSource = items;
				AutoCompleteBox.SelectedIndex = items.IndexOf( selectedText );
				if( AutoCompleteBox.SelectedIndex < 0 )
				{
					AutoCompleteBox.SelectedIndex = 0;
				}
				AutoCompleteBox.InvalidateProperty( ItemsControl.ItemsSourceProperty );
			}
		}

		private void AutoCompleteBox_MouseDoubleClick( object sender, MouseButtonEventArgs e )
		{
			DependencyObject obj = (DependencyObject)e.OriginalSource;

			while( obj != null && obj != AutoCompleteBox )
			{
				if( obj.GetType() == typeof( ListViewItem ) )
				{
					InsertText();
					e.Handled = true;
					CloseAutoCompleteBox();
					break;
				}
				obj = VisualTreeHelper.GetParent( obj );
			}
		}

		private void AutoCompletePopup_Closed( object sender, EventArgs e )
		{
			FilterText = string.Empty;
		}

		private void InsertText()
		{
			var filter = FilterText ?? string.Empty;
			var insertText = filter + FilteredItems.ElementAt( AutoCompleteBox.SelectedIndex ).Substring( filter.Length );
			var currentCaret = AssociatedObject.CaretIndex;

			string newText = AssociatedObject.Text;
			newText = newText.Remove( currentCaret - filter.Length, filter.Length );
			currentCaret -= filter.Length;
			newText = newText.Insert( currentCaret, insertText );

			AssociatedObject.Text = newText;
			AssociatedObject.CaretIndex = currentCaret + insertText.Length;
		}

		public IEnumerable<string> Hashtags
		{
			get { return (IEnumerable<string>)GetValue( ItemsSourceProperty ); }
			set { SetValue( ItemsSourceProperty, value ); }
		}

		public IEnumerable<string> Users
		{
			get { return (IEnumerable<string>)GetValue( UsersProperty ); }
			set { SetValue( UsersProperty, value ); }
		}

		private IEnumerable<string> FilteredHashtags
		{
			get
			{
				var items = Hashtags?.Distinct() ?? Enumerable.Empty<string>();

				if( !string.IsNullOrWhiteSpace( FilterText ) )
				{
					items = items.Where( it => it.StartsWith( FilterText, StringComparison.OrdinalIgnoreCase ) );
				}

				return items.OrderBy( x => x );
			}
		}

		private IEnumerable<string> FilteredItems => Mode == SourceMode.Hashtags ? FilteredHashtags : FilteredUsers;

		private IEnumerable<string> FilteredUsers
		{
			get
			{
				var items = Users?.Distinct() ?? Enumerable.Empty<string>();

				if( !string.IsNullOrWhiteSpace( FilterText ) )
				{
					items = items.Where( it => it.StartsWith( FilterText, StringComparison.OrdinalIgnoreCase ) );
				}

				return items.OrderBy( x => x );
			}
		}

		public static readonly DependencyProperty ItemsSourceProperty =
			DependencyProperty.Register( "Hashtags", typeof( IEnumerable<string> ), typeof( AutoComplete ),
				new PropertyMetadata( new string[] {} ) );

		public static readonly DependencyProperty UsersProperty =
			DependencyProperty.Register( "Users", typeof( IEnumerable<string> ), typeof( AutoComplete ), new PropertyMetadata( new string[] {} ) );

		private readonly ListView AutoCompleteBox;
		private readonly Popup AutoCompletePopup;
		private string FilterText;
		private SourceMode Mode = SourceMode.None;
	}
}