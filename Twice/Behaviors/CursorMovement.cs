﻿using System.Windows;
using System.Windows.Controls;
using Twice.ViewModels;

namespace Twice.Behaviors
{
	/// <summary>
	///     Allows manipulation of the cursor of a text box from a view model.
	/// </summary>
	internal class CursorMovement : BehaviorBase<TextBox>
	{
		private void Controller_ScrollToEnd( object sender, System.EventArgs e )
		{
			AssociatedObject.ScrollToEnd();
			AssociatedObject.SelectionStart = AssociatedObject.Text.Length;
		}

		private static void OnControllerChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
		{
			( d as CursorMovement )?.OnControllerChanged( e.NewValue as ICursorController );
		}

		private void OnControllerChanged( ICursorController controller )
		{
			if( Controller != null )
			{
				Controller.ScrollToEnd -= Controller_ScrollToEnd;
			}

			Controller = controller;
			if( Controller != null )
			{
				Controller.ScrollToEnd += Controller_ScrollToEnd;
			}
		}

		public static readonly DependencyProperty ControllerProperty =
			DependencyProperty.Register( "Controller", typeof( ICursorController ), typeof( CursorMovement ), new PropertyMetadata( null, OnControllerChanged ) );

		public ICursorController Controller
		{
			get => (ICursorController)GetValue( ControllerProperty );
			set => SetValue( ControllerProperty, value );
		}
	}
}