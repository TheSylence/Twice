using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interactivity;
using WPFTextBoxAutoComplete;

namespace Twice.Behaviors
{
	internal class AutoComplete : Behavior<TextBox>
	{
		public AutoComplete()
		{
			AutoCompleteBox = new TextBox
			{
				HorizontalAlignment = HorizontalAlignment.Stretch,
				VerticalAlignment = VerticalAlignment.Center
			};
			AutoCompleteBehavior.SetAutoCompleteStringComparison( AutoCompleteBox, StringComparison.OrdinalIgnoreCase );

			AutoCompletePopup = new Popup
			{
				Child = AutoCompleteBox,
				StaysOpen = false,
				Placement = PlacementMode.Center
			};

			AutoCompleteBox.PreviewKeyDown += AutoCompleteBox_PreviewKeyDown;
			AutoCompleteBox.PreviewTextInput += AutoCompleteBox_PreviewTextInput;
		}

		protected override void OnAttached()
		{
			base.OnAttached();

			AssociatedObject.PreviewTextInput += AssociatedObject_TextInput;
			AutoCompletePopup.PlacementTarget = AssociatedObject;

			var binding = new Binding( "ActualWidth" )
			{
				Source = AssociatedObject
			};
			AutoCompletePopup.SetBinding( FrameworkElement.WidthProperty, binding );
		}

		private static void OnItemsSourceChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
		{
			var auto = d as AutoComplete;
			auto?.OnItemsSourceChanged( e.NewValue as IEnumerable<string> );
		}

		private void AssociatedObject_TextInput( object sender, TextCompositionEventArgs e )
		{
			if( string.IsNullOrEmpty( TriggerChar ) )
			{
				return;
			}

			var text = e.Text;
			if( text.StartsWith( TriggerChar ) )
			{
				AutoCompletePopup.IsOpen = true;
				AutoCompleteBox.Focus();
				AutoCompleteBox.Text = string.Empty;
				e.Handled = true;
			}
		}

		private void AutoCompleteBox_PreviewKeyDown( object sender, KeyEventArgs e )
		{
			if( e.Key == Key.Escape )
			{
				AutoCompletePopup.IsOpen = false;
			}
			else if( e.Key == Key.Return )
			{
				var insertText = $"{TriggerChar}{AutoCompleteBox.Text}";
				var currentCaret = AssociatedObject.CaretIndex;

				AssociatedObject.Text = AssociatedObject.Text.Insert( currentCaret , insertText );

				AutoCompletePopup.IsOpen = false;
				e.Handled = true;
				
				AssociatedObject.Focus();
				AssociatedObject.CaretIndex = currentCaret + insertText.Length;
			}
		}

		private void AutoCompleteBox_PreviewTextInput( object sender, TextCompositionEventArgs e )
		{
			if( e.Text.Equals( TriggerChar ) )
			{
				e.Handled = true;
			}
		}

		private void OnItemsSourceChanged( IEnumerable<string> items )
		{
			AutoCompleteBehavior.SetAutoCompleteItemsSource( AutoCompleteBox, items );
		}

		public IEnumerable<string> ItemsSource
		{
			get { return (IEnumerable<string>)GetValue( ItemsSourceProperty ); }
			set { SetValue( ItemsSourceProperty, value ); }
		}

		public string TriggerChar
		{
			get { return (string)GetValue( TriggerCharProperty ); }
			set { SetValue( TriggerCharProperty, value ); }
		}

		public static readonly DependencyProperty ItemsSourceProperty =
			DependencyProperty.Register( "ItemsSource", typeof( IEnumerable<string> ), typeof( AutoComplete ),
				new PropertyMetadata( new string[] {}, OnItemsSourceChanged ) );

		public static readonly DependencyProperty TriggerCharProperty =
			DependencyProperty.Register( "TriggerChar", typeof( string ), typeof( AutoComplete ), new PropertyMetadata( string.Empty ) );

		private readonly TextBox AutoCompleteBox;
		private readonly Popup AutoCompletePopup;
	}
}