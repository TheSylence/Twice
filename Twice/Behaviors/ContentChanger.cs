using System.Windows;
using Twice.ViewModels.Dialogs;

namespace Twice.Behaviors
{
	internal class ContentChanger : BehaviorBase<Window>
	{
		private void DataContextChanger_DataContextChange( object sender, ContentChangeEventArgs e )
		{
			AssociatedObject.Content = e.NewContent;
		}

		private static void OnContextChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
		{
			var changer = d as ContentChanger;
			changer?.OnContextChanged( e.NewValue as IContentChanger );
		}

		private void OnContextChanged( IContentChanger dataContextChanger )
		{
			if( CurrentContentChanger != null )
			{
				CurrentContentChanger.ContentChange -= DataContextChanger_DataContextChange;
			}

			CurrentContentChanger = dataContextChanger;
			if( CurrentContentChanger != null )
			{
				CurrentContentChanger.ContentChange += DataContextChanger_DataContextChange;
			}
		}

		public static readonly DependencyProperty ContextChangerProperty =
			DependencyProperty.Register( "ContextChanger", typeof( IContentChanger ), typeof( ContentChanger ), new PropertyMetadata( null, OnContextChanged ) );

		public IContentChanger ContextChanger
		{
			get => (IContentChanger)GetValue( ContextChangerProperty );
			set => SetValue( ContextChangerProperty, value );
		}

		private IContentChanger CurrentContentChanger;
	}
}