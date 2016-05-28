using GalaSoft.MvvmLight.Messaging;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using Twice.Messages;
using Twice.ViewModels.Columns;
using Twice.Views;

namespace Twice.Controls
{
	[ExcludeFromCodeCoverage]
	internal class ResizeThumb : Thumb
	{
		public ResizeThumb()
		{
			DragStarted += ResizeThumb_DragStarted;
			DragDelta += ResizeThumb_DragDelta;
			DragCompleted += ResizeThumb_DragCompleted;
			MouseDown += ResizeThumb_MouseDown;
			MouseUp += ResizeThumb_MouseUp;

			Messenger.Default.Register<ColumnLockMessage>( this, OnColumnLock );
		}

		private void OnColumnLock( ColumnLockMessage msg )
		{
			IsEnabled = !msg.Locked;
		}

		private void ResizeThumb_DragCompleted( object sender, DragCompletedEventArgs e )
		{
			if( Adorner != null )
			{
				AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer( ParentItem );
				adornerLayer?.Remove( Adorner );

				Adorner = null;
				if( !e.Canceled )
				{
					Messenger.Default.Send( new DragMessage( this, false ) );
				}
			}

			e.Handled = true;
		}

		private void ResizeThumb_DragDelta( object sender, DragDeltaEventArgs e )
		{
			if( Item != null )
			{
				switch( HorizontalAlignment )
				{
				case HorizontalAlignment.Right:
					var deltaHorizontal = Math.Min( -e.HorizontalChange, Item.Width /*- Item.MinWidth*/ );
					Item.Width -= deltaHorizontal;
					break;
				}
			}

			e.Handled = true;
		}

		private void ResizeThumb_DragStarted( object sender, DragStartedEventArgs e )
		{
			Item = DataContext as IColumnViewModel;

			if( Item != null )
			{
				ParentItem = VisualTreeWalker.FindParent<ColumnHeader>( this );

				if( ParentItem != null )
				{
					AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer( ParentItem );
					if( adornerLayer != null )
					{
						Adorner = new ResizeAdorner( ParentItem );
						adornerLayer.Add( Adorner );
					}
				}

				Messenger.Default.Send( new DragMessage( this, true ) );
			}

			e.Handled = true;
		}

		private void ResizeThumb_MouseDown( object sender, MouseButtonEventArgs e )
		{
			Messenger.Default.Send( new DragMessage( this, true ) );
		}

		private void ResizeThumb_MouseUp( object sender, MouseButtonEventArgs e )
		{
			Messenger.Default.Send( new DragMessage( this, false ) );
		}

		private Adorner Adorner;
		private IColumnViewModel Item;
		private ColumnHeader ParentItem;
	}
}