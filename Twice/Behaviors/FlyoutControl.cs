using Fody;
using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro.Controls;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Interactivity;
using Twice.Messages;
using Twice.ViewModels;

namespace Twice.Behaviors
{
	[ExcludeFromCodeCoverage]
	internal class FlyoutControl : Behavior<Flyout>
	{
		protected override void OnAttached()
		{
			base.OnAttached();

			Messenger.Default.Register<FlyoutMessage>( this, OnFlyoutMessage );
		}

		[ConfigureAwait( false )]
		private async void OnFlyoutMessage( FlyoutMessage message )
		{
			if( !message.Name.Equals( Name ) )
			{
				return;
			}

			switch( message.Action )
			{
			case FlyoutAction.Open:
				AssociatedObject.IsOpen = true;
				break;

			case FlyoutAction.Close:
				AssociatedObject.IsOpen = false;
				break;

			case FlyoutAction.Toggle:
				AssociatedObject.IsOpen = !AssociatedObject.IsOpen;
				break;
			}

			if( AssociatedObject.IsOpen )
			{
				if( message.DataContext != null )
				{
					AssociatedObject.DataContext = message.DataContext;
				}

				var resetable = AssociatedObject.DataContext as IResetable;
				if( resetable != null )
				{
					await resetable.Reset();
				}
			}
		}

		public static readonly DependencyProperty NameProperty = DependencyProperty.Register( "Name", typeof(string),
			typeof(FlyoutControl), new PropertyMetadata( null ) );

		public string Name
		{
			get { return (string)GetValue( NameProperty ); }
			set { SetValue( NameProperty, value ); }
		}
	}
}