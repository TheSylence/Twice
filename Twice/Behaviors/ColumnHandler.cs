using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using Twice.ViewModels.Columns;

namespace Twice.Behaviors
{
	[ExcludeFromCodeCoverage]
	internal class ColumnHandler : Behavior<ItemsControl>
	{
		private static void OnDispatcherChanged( DependencyObject obj, DependencyPropertyChangedEventArgs e )
		{
			var handler = obj as ColumnHandler;
			handler?.OnDispatcherChanged( e.NewValue as IColumnActionDispatcher, e.OldValue as IColumnActionDispatcher );
		}

		private void Dispatcher_HeaderClicked( object sender, EventArgs e )
		{
			ContentPresenter itemcontainer = AssociatedObject.ItemContainerGenerator.ContainerFromIndex( 0 ) as ContentPresenter;
			Debug.Assert( itemcontainer != null, "itemcontainer != null" );
			itemcontainer.BringIntoView();
		}

		private void OnDispatcherChanged( IColumnActionDispatcher newDispatcher, IColumnActionDispatcher oldDispatcher )
		{
			if( oldDispatcher != null )
			{
				oldDispatcher.HeaderClicked -= Dispatcher_HeaderClicked;
			}

			if( newDispatcher != null )
			{
				newDispatcher.HeaderClicked += Dispatcher_HeaderClicked;
			}
		}

		public static readonly DependencyProperty ActionDispatcherProperty =
			DependencyProperty.Register( "ActionDispatcher", typeof(IColumnActionDispatcher), typeof(ColumnHandler),
				new PropertyMetadata( null, OnDispatcherChanged ) );

		public IColumnActionDispatcher ActionDispatcher
		{
			get { return (IColumnActionDispatcher)GetValue( ActionDispatcherProperty ); }
			set { SetValue( ActionDispatcherProperty, value ); }
		}
	}
}