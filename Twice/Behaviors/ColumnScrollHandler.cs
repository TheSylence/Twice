using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using Twice.ViewModels.Columns;

namespace Twice.Behaviors
{
	[ExcludeFromCodeCoverage]
	internal class ColumnScrollHandler : BehaviorBase<ScrollViewer>
	{
		protected override void OnAttached()
		{
			base.OnAttached();

			AssociatedObject.ScrollChanged += AssociatedObject_ScrollChanged;
		}

		private static void OnDispatcherChanged( DependencyObject obj, DependencyPropertyChangedEventArgs e )
		{
			var handler = obj as ColumnScrollHandler;
			handler?.OnDispatcherChanged( e.NewValue as IColumnActionDispatcher, e.OldValue as IColumnActionDispatcher );
		}

		private void AssociatedObject_ScrollChanged( object sender, ScrollChangedEventArgs e )
		{
			if( e.VerticalChange <= 0 )
			{
				HandledRange.Reset();
				return;
			}

			if( HandledRange.Contains( e.VerticalOffset ) )
			{
				return;
			}

			if( e.VerticalOffset + e.ViewportHeight >= e.ExtentHeight * 0.8 )
			{
				HandledRange.Start = e.VerticalOffset;
				HandledRange.End = e.ExtentHeight;
				ActionDispatcher?.OnBottomReached();
			}
		}

		private void Dispatcher_HeaderClicked( object sender, EventArgs e )
		{
			AssociatedObject.ScrollToTop();
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

		public IColumnActionDispatcher ActionDispatcher
		{
			get { return (IColumnActionDispatcher)GetValue( ActionDispatcherProperty ); }
			set { SetValue( ActionDispatcherProperty, value ); }
		}

		public static readonly DependencyProperty ActionDispatcherProperty =
			DependencyProperty.Register( "ActionDispatcher", typeof( IColumnActionDispatcher ), typeof( ColumnScrollHandler ),
				new PropertyMetadata( null, OnDispatcherChanged ) );

		private readonly Range HandledRange = new Range();

		private class Range
		{
			public Range()
			{
				Reset();
			}

			public bool Contains( double v )
			{
				return Start <= v && End >= v;
			}

			public void Reset()
			{
				Start = double.MaxValue;
				End = double.MinValue;
			}

			public double End;
			public double Start;
		}
	}
}